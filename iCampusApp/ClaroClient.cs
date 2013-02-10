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
using System.Threading.Tasks;

#if !DEBUG
    using Microsoft.Phone.Net.NetworkInformation;
#endif

namespace ClarolineApp
{
    public class ClaroClient : INotifyPropertyChanged
    {

        private static ClaroClient _instance;
        public static ClaroClient instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClaroClient();
                }
                return _instance;
            }
        }

        AppSettings settings;

        CookieContainer cookies;
        
        private DateTime _cookieCreation;
        
        DateTime cookieCreation
        {
            get
            {
                return _cookieCreation;
            }
            set
            {
                if (!_cookieCreation.Equals(value))
                {
                    _cookieCreation = value;
                    NotifyPropertyChanged("cookieCreation");
                    NotifyPropertyChanged("isExpired");
                }
            }
        }

        public Boolean isExpired
        {
            get
            {
                return (cookieCreation.AddHours(1.0).CompareTo(DateTime.Now) < 0);
            }
        }

        private Exception _lastException;

        public Exception lastException
        {
            get { return _lastException; }
            set
            {
                if (!_lastException.Equals(value))
                {
                    _lastException = value;
                    NotifyPropertyChanged("lastException");
                }
            }
        }
        

        public ClaroClient()
        {
            //Environment
            settings = new AppSettings();

            //WebRequesting stuff
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
        }

        private async Task<HttpWebRequest> getClientAsync(PostDataWriter args)
        {
            //Data request
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(args.GetURL());
            client.Method = "POST";
            client.CookieContainer = cookies;
            client.ContentType = "application/x-www-form-urlencoded";
            client.AllowAutoRedirect = false;

            Stream postStream = await client.GetRequestStreamAsync();
            String postData = args.GetPostDataString();

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write to the request stream.
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();

            return client;
        }

        public async Task<string> makeOperationAsync(SupportedModules module, SupportedMethods method, Cours reqCours = null, int resID = -1)
        {
            if (IsNetworkAvailable())
            {
                PostDataWriter args = new PostDataWriter() { module = module, method = method, cidReq = reqCours, resId = resID };

                String strContent = "";
                HttpWebResponse response = null;
                Stream responseStream = null;
                try
                {
                    HttpWebRequest client = await getClientAsync(args);
                    response = (HttpWebResponse)await client.GetResponseAsync();
                    responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                    strContent = sr.ReadToEnd();
                    responseStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                    if (response != null)
                    {
                        response.Close();
                    }
                }
                return strContent;
            }
            else
            {
                lastException = new NetworkException("Network Unavailable");
                return null;
            }
        }

        private async Task<bool> GetSessionCookieAsync()
        {
            String result = await makeOperationAsync(SupportedModules.NOMOD, SupportedMethods.authenticate);
            if (result.Equals(String.Empty))
            {
                cookieCreation = DateTime.Now;
                return true;
            }
            else
            {
                lastException = new AuthenticationException("Authentication Fails");
                return false;
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

        public async Task<Boolean> isValidAccountAsync()
        {
            if (isExpired && IsNetworkAvailable())
            {
                return await GetSessionCookieAsync();
            }
            else
            {
                return true;
            }
        }

        public Boolean isValidAccountWithoutWaiting()
        {
            return !isExpired;
        }

        [Obsolete]
        public void ResponseCallback(IAsyncResult ar)
        {/*
            String strContent = "";
            PostDataWriter args = (PostDataWriter)ar.AsyncState;
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
                        String ptAuth = "";
                        String ptAnon = "";
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
                                    case "platformTextAuth":
                                        ptAuth = reader.ReadAsString();
                                        break;
                                    case "platformTextAnonym":
                                        ptAnon = reader.ReadAsString();
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
                            settings.PlatformTextAuthSetting = ptAuth;
                            settings.PlatformTextAnonSetting = ptAnon;
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
                                VM.ClearCoursList();
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
                                VM.ClearDocsOfCours(args.cidReq);
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
                                    VM.AddAnnonce(annonce);
                                }
                                VM.ClearAnnsOfCours(args.cidReq);
                                pulse(Updating);
                            });
                        }
                        break;

                    case AllowedOperations.getSingleAnnounce:
                        setProgressIndicator(true, String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.CoursPage_Ann_PI));
                        Annonce singleAnnonce = JsonConvert.DeserializeObject<Annonce>(strContent);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            singleAnnonce.Cours = args.cidReq;
                            VM.AddAnnonce(singleAnnonce);
                            pulse(Updating);
                        });
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
                                                                upAnn.upToDateContent = false;
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
                            cookieCreation = DateTime.Now;
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
        */}

        public void invalidateClient()
        {
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
            settings.UserSetting = new User();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        #endregion

        class NetworkException : WebException {
            public NetworkException(string message)
                :base(message)
            { }
        }
        class AuthenticationException : WebException {
            public AuthenticationException(string message)
                :base(message)
            { }
        }

    }

}
