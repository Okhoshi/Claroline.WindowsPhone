using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using ClarolineApp.Model;
using ClarolineApp.ViewModel;
using Newtonsoft.Json;
using HtmlAgilityPack;
using ClarolineApp.Settings;
using ClarolineApp.Languages;

namespace ClarolineApp
{
    public class ClaroClient : INotifyPropertyChanged
    {
        AppSettings settings;
        iCampusViewModel VM;

        Mutex Requesting;
        Mutex Updating;
        BackgroundWorker bw;
        private int Waiting = 0;
        private int Waiting2 = 0;

        CookieContainer cookies;
        DateTime CookieCreation;
        public Boolean isExpired
        {
            get
            {
                return (CookieCreation.AddHours(1.0).CompareTo(DateTime.Now) < 0);
            }
        }

        public ClaroClient()
        {
            //Environment
            VM = App.ViewModel;
            settings = new AppSettings();

            //Threading stuff
            Requesting = new Mutex(false, "Request");
            Updating = new Mutex(false, "Update");
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(DoWork);

            //WebRequesting stuff
            CookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
        }

        private HttpWebRequest getClient(Boolean forAuth = false)
        {
            //Data request
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(settings.DomainSetting + ((forAuth) ? settings.AuthPageSetting : settings.WebServiceSetting));
            client.Method = "POST";
            client.CookieContainer = cookies;
            client.ContentType = "application/x-www-form-urlencoded";
            client.AllowAutoRedirect = false;
            return client;
        }

        public void makeOperation(AllowedOperations op, Cours reqCours = null)
        {
            if (IsNetworkAvailable())
            {
                if (!bw.IsBusy)
                {
                    CallbackArgs args;
                    switch (op)
                    {
                        case AllowedOperations.getCourseToolList:
                        case AllowedOperations.getDocList:
                        case AllowedOperations.getAnnounceList:
                            if (reqCours == null)
                            {
                                return;
                            }
                            args = new CallbackArgs() { Request = getClient(), cidReq = reqCours, operation = op };
                            bw.RunWorkerAsync(args);
                            break;
                        case AllowedOperations.getUserData:
                        case AllowedOperations.getCourseList:
                        case AllowedOperations.getUpdates:
                            args = new CallbackArgs() { Request = getClient(), operation = op };
                            bw.RunWorkerAsync(args);
                            break;
                        case AllowedOperations.updateCompleteCourse:
                            if (reqCours == null)
                            {
                                return;
                            }
                            args = new CallbackArgs() { cidReq = reqCours, operation = op };
                            bw.RunWorkerAsync(args);
                            break;
                    }
                    Debug.WriteLine("Appel de la méthode " + op.ToString());
                }
            }
            else
            {
                MessageBox.Show(AppLanguage.ErrorMessage_NetworkUnavailable);
                NotifyPropertyChanged("Failure");
            }
        }

        public bool IsNetworkAvailable()
        {
#if !DEBUG
            return (DeviceNetworkInformation.IsNetworkAvailable && (DeviceNetworkInformation.IsWiFiEnabled || (DeviceNetworkInformation.IsCellularDataEnabled && settings.CellularDataEnabledSetting)));
#else
            return settings.CellularDataEnabledSetting;
#endif
        }

        public Boolean isValidAccount()
        {
            if (isExpired && !bw.IsBusy && IsNetworkAvailable())
            {
                CallbackArgs args = new CallbackArgs() { operation = AllowedOperations.authenticate };
                bw.RunWorkerAsync(args);
            }
            return !isExpired;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            CallbackArgs args;

            switch (((CallbackArgs)e.Argument).operation)
            {
                case AllowedOperations.authenticate:
                    getSessionCookie();
                    break;
                case AllowedOperations.updateCompleteCourse:
                    if (isExpired)
                    {
                        getSessionCookie();
                        wait();
                        if (bw.CancellationPending) return;
                    }
                    setProgressIndicator(true, AppLanguage.ProgressBar_Connecting);
                    args = new CallbackArgs() { Request = getClient(), operation = AllowedOperations.getCourseToolList, cidReq = ((CallbackArgs)e.Argument).cidReq };
                    args.Request.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
                    wait();
                    if (bw.CancellationPending || !((CallbackArgs)e.Argument).cidReq.isDnL) return;
                    setProgressIndicator(true, AppLanguage.ProgressBar_Connecting);
                    args = new CallbackArgs() { Request = getClient(), operation = AllowedOperations.getDocList, cidReq = ((CallbackArgs)e.Argument).cidReq };
                    args.Request.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
                    if (bw.CancellationPending || !((CallbackArgs)e.Argument).cidReq.isAnn) return;
                    setProgressIndicator(true, AppLanguage.ProgressBar_Connecting);
                    args = new CallbackArgs() { Request = getClient(), operation = AllowedOperations.getAnnounceList, cidReq = ((CallbackArgs)e.Argument).cidReq };
                    args.Request.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
                    wait();
                    wait();
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        ((CallbackArgs)e.Argument).cidReq.isLoaded = true;
                    });
                    break;
                default:
                    if (isExpired)
                    {
                        getSessionCookie();
                        wait();
                        if (bw.CancellationPending) return;
                    }
                    setProgressIndicator(true, AppLanguage.ProgressBar_Connecting);
                    args = e.Argument as CallbackArgs;
                    args.Request.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
                    //wait();
                    //authNotify();
                    break;
            }
        }

        private void RequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = ((CallbackArgs)ar.AsyncState).Request;
            String postData = ((CallbackArgs)ar.AsyncState).ToString();
            AllowedOperations op = ((CallbackArgs)ar.AsyncState).operation;
            // End the operation
            Stream postStream = request.EndGetRequestStream(ar);

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write to the request stream.
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();
            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(ResponseCallback), ar.AsyncState);
        }

        public void ResponseCallback(IAsyncResult ar)
        {
            String strContent = "";
            CallbackArgs args = (CallbackArgs)ar.AsyncState;
            //Récupération de la requete web (object HttpWebRequest)
            HttpWebRequest Request = args.Request;
            AllowedOperations op = args.operation;
            //Récupération de la réponse Web
            try
            {
                HttpWebResponse Response = (HttpWebResponse)Request.EndGetResponse(ar);
                Stream responseStream = Response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                strContent = sr.ReadToEnd();
                responseStream.Close();
                Response.Close();

                switch (op)
                {
                    case AllowedOperations.getUserData:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.SettingsPage_User_PI));
                        User connectedUser = JsonConvert.DeserializeObject<User>(strContent);
                        StringReader str = new StringReader(strContent);
                        String institution = "";
                        String platform = "";
                        JsonTextReader reader = new JsonTextReader(str);
                        while (reader.Read())
                        {
                            if (reader.Value != null)
                            {
                                switch (reader.Value.ToString())
                                {
                                    case "institutionName":
                                        institution = reader.ReadAsString();
                                        break;
                                    case "platformName":
                                        platform = reader.ReadAsString();
                                        break;
                                    default:
                                        continue;
                                }
                            }
                        }
                        reader.Close();
                        str.Close();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            settings.UserSetting.setUser(connectedUser);
                            settings.IntituteSetting = institution;
                            settings.PlatformSetting = platform;
                        });
                        authNotify();
                        break;

                    case AllowedOperations.getCourseList:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.MainPage_Cours_PI));
                        List<Cours> Courses = JsonConvert.DeserializeObject<List<Cours>>(strContent);
                        if (Courses.Count > 0)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                foreach (Cours cours in Courses)
                                {
                                    VM.AddCours(cours);
                                }
                                pulse(Updating);
                            });
                        }
                        break;

                    case AllowedOperations.getCourseToolList:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.MainPage_Cours_PI));
                        Cours result = JsonConvert.DeserializeObject<Cours>(strContent);
                        args.cidReq.annNotif = result.annNotif;
                        args.cidReq.dnlNotif = result.dnlNotif;
                        args.cidReq.isAnn = result.isAnn;
                        args.cidReq.isDnL = result.isDnL;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            VM.AddCours(args.cidReq);
                        });
                        break;

                    case AllowedOperations.getDocList:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.CoursPage_Doc_PI));
                        List<Documents> Documents = JsonConvert.DeserializeObject<List<Documents>>(strContent);
                        if (Documents.Count > 0 && Documents.First().Cours.Equals(args.cidReq))
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                foreach (Documents doc in Documents)
                                {
                                    doc.Cours = args.cidReq;
                                    VM.AddDocument(doc);
                                }
                                pulse(Updating);
                            });
                        }
                        break;

                    case AllowedOperations.getAnnounceList:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.CoursPage_Ann_PI));
                        List<Annonce> Annonces = JsonConvert.DeserializeObject<List<Annonce>>(strContent);
                        if (Annonces.Count > 0 && Annonces.First().Cours.Equals(args.cidReq))
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                foreach (Annonce annonce in Annonces)
                                {
                                    annonce.Cours = args.cidReq;
                                    if(annonce.content != null)
                                        annonce.content = (new HtmlToText()).ConvertHtml(annonce.content);
                                    VM.AddAnnonce(annonce);
                                }
                                pulse(Updating);
                            });
                        }
                        break;

                    case AllowedOperations.getUpdates:
                        setProgressIndicator(true, AppLanguage.ProgressBar_Update);
                        Debug.WriteLine(strContent);
                        if (strContent != "[]")
                        {
                            Dictionary<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>> Updates;
                            Updates = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>>>(strContent);
                            foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>> course in Updates)
                            {
                                Cours upCours = VM.AllCours.FirstOrDefault(cours => cours.sysCode.Equals(course.Key));
                                if (upCours == null)
                                {
                                    makeOperation(AllowedOperations.getCourseList);
                                    wait(Updating);
                                    upCours = VM.AllCours.FirstOrDefault(cours => cours.sysCode.Equals(course.Key));
                                    if (upCours != null)
                                    {
                                        makeOperation(AllowedOperations.updateCompleteCourse, upCours);
                                    }
                                    continue;
                                }
                                else
                                {
                                    foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, String>>> tool in course.Value)
                                    {
                                        switch (tool.Key)
                                        {
                                            case "CLANN":
                                                if (!upCours.isAnn)
                                                {
                                                    makeOperation(AllowedOperations.getCourseToolList, upCours);
                                                    wait(Updating);
                                                    if (upCours.isAnn)
                                                    {
                                                        makeOperation(AllowedOperations.getAnnounceList, upCours);
                                                    }
                                                    continue;
                                                }
                                                else
                                                {
                                                    foreach (KeyValuePair<String, Dictionary<String, String>> ressource in tool.Value)
                                                    {
                                                        Annonce upAnn = VM.AnnByCours[course.Key].FirstOrDefault(announce => announce.ressourceId == int.Parse(ressource.Key));
                                                        if (upAnn == null)
                                                        {
                                                            makeOperation(AllowedOperations.getAnnounceList, upCours);
                                                        }
                                                        else
                                                        {
                                                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                            {
                                                                upAnn.date = DateTime.Parse(ressource.Value["date"]);
                                                                upAnn.notified = true;
                                                                VM.AddAnnonce(upAnn);
                                                                pulse(Updating);
                                                            });
                                                        }
                                                    }
                                                }
                                                break;
                                            case "CLDOC":
                                                if (!upCours.isDnL)
                                                {
                                                    makeOperation(AllowedOperations.getCourseToolList, upCours);
                                                    wait(Updating);
                                                    if (upCours.isDnL)
                                                    {
                                                        makeOperation(AllowedOperations.getDocList, upCours);
                                                    }
                                                    continue;
                                                }
                                                else
                                                {
                                                    foreach (KeyValuePair<String, Dictionary<String, String>> ressource in tool.Value)
                                                    {
                                                        Documents upDoc = VM.DocByCours[course.Key].FirstOrDefault(doc => doc.path == ressource.Key);
                                                        if (upDoc == null)
                                                        {
                                                            makeOperation(AllowedOperations.getDocList, upCours);
                                                        }
                                                        else
                                                        {
                                                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                            {
                                                                upDoc.date = DateTime.Parse(ressource.Value["date"]);
                                                                upDoc.notified = true;
                                                                VM.AddDocument(upDoc);
                                                                pulse(Updating);
                                                            });
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case AllowedOperations.authenticate:
                        if (!strContent.Contains(settings.AuthPageSetting))
                        {
                            CookieCreation = DateTime.Now;
                            authNotify();
                        }
                        else
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show(AppLanguage.ErrorMessage_AuthFailed);
                            });
                            bw.CancelAsync();
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    NotifyPropertyChanged("Failure");
                    MessageBox.Show(String.Format(AppLanguage.ErrorMessage_ServerReturnCode,
                                                    (int)((HttpWebResponse)ex.Response).StatusCode,
                                                    ((HttpWebResponse)ex.Response).StatusDescription));
                });
                bw.CancelAsync();
                authNotify();
                Request.Abort();
            }
            catch (JsonReaderException ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    NotifyPropertyChanged("Failure");
                    MessageBox.Show(AppLanguage.ErrorMessage_UnreadableJson);
                });
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(strContent);
            }
            finally
            {
                setProgressIndicator(false);
                pulse();
            }
            return;
        }

        private void getSessionCookie()
        {
            CallbackArgs args = new CallbackArgs() { Request = getClient(true), login = settings.UsernameSetting, password = settings.PasswordSetting, operation = AllowedOperations.authenticate };
            setProgressIndicator(true, AppLanguage.ProgressBar_Connecting);
            args.Request.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
        }

        private void wait(Mutex mutex = null)
        {
            if (mutex == null) mutex = Requesting;
            lock (mutex)
            {
                Debug.WriteLine("Wait Pulse " + mutex.Equals(Requesting));
                if (mutex == Requesting)
                {
                    Waiting++;
                }
                else
                {
                    Waiting2++;
                }
                Monitor.Wait(mutex);
            }
        }

        private void pulse(Mutex mutex = null)
        {
            if (mutex == null) mutex = Requesting;
            if (((mutex == Requesting)?Waiting:Waiting2) > 0)
            {
                lock (mutex)
                {
                    Monitor.PulseAll(mutex);
                    if (mutex == Requesting)
                    {
                        Waiting--;
                    }
                    else
                    {
                        Waiting2--;
                    }
                    Debug.WriteLine("PULSING " + mutex.Equals(Requesting));
                }
            }
        }

        private void authNotify()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NotifyPropertyChanged("isExpired");
            });
        }

        public void invalidateClient()
        {
            CookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
            settings.UserSetting = new User();
            authNotify();
        }

        private void setProgressIndicator(Boolean visible, String message = "")
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (App.currentProgressInd != null)
                {
                    App.currentProgressInd.Text = message;
                    App.currentProgressInd.IsVisible = visible;
                }
            });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

    public class CallbackArgs
    {
        public HttpWebRequest Request;
        public AllowedOperations operation;
        public Cours cidReq;
        public String login;
        public String password;

        public override string ToString()
        {
            switch (operation)
            {
                case AllowedOperations.authenticate:
                    return "login=" + login + "&password=" + password;
                case AllowedOperations.getUserData:
                    return "Method=getUserData";
                case AllowedOperations.getCourseList:
                    return "Method=getCourseList";
                case AllowedOperations.getCourseToolList:
                    return "Method=getCourseToolList&cidReq=" + cidReq.sysCode;
                case AllowedOperations.getDocList:
                    return "Method=getDocList&cidReq=" + cidReq.sysCode;
                case AllowedOperations.getAnnounceList:
                    return "Method=getAnnounceList&cidReq=" + cidReq.sysCode;
                case AllowedOperations.getUpdates:
                    return "Method=getUpdates";
                default:
                    return base.ToString();
            }
        }
    }

    public enum AllowedOperations
    {
        authenticate,
        getUserData,
        getCourseList,
        getCourseToolList,
        getDocList,
        getAnnounceList,
        updateCompleteCourse,
        getUpdates
    }
}
