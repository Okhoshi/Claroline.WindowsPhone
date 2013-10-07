using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using iCampusApp.Model;
using System.Diagnostics;

namespace iCampusApp
{
    public class Connect
    {
        public RequestState _rs = null;
        static public CookieContainer cookies = new CookieContainer();

        public void Sync(string url)
        {
            if (String.IsNullOrEmpty(url)) url = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create((new AppSettings()).MainSiteSetting + url);
            // Création de l'objet état
            //On ajoute la requete dans l'objet état pour pouvoir le récupérer dans la callback
            req.Method = "POST";
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
//            Debug.WriteLine(url);
            if (App.currentProgressInd != null)
            {
                App.currentProgressInd.Text = "Requête iCampus...";
                App.currentProgressInd.IsVisible = true;
            }
                req.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), req);
        }

        private void GetRequestStreamCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            AppSettings set = new AppSettings();
            String postData = "login=" + set.UsernameSetting + "&password=" + set.PasswordSetting;
            // End the operation
            Stream postStream = request.EndGetRequestStream(ar);

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.GetEncoding("iso-8859-1").GetBytes(postData);

            // Write to the request stream.
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();
            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(ResponseSyncCallback), request);
        }

        //private void ResponseConnectCallback(IAsyncResult ar)
        //{
        //    //Récupération de la requete web (object HttpWebRequest)
        //    HttpWebRequest req = (HttpWebRequest)ar.AsyncState;
        //    //Récupération de la réponse Web	
        //    try
        //    {
        //        HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);

        //        Stream responseStream = resp.GetResponseStream();
        //        StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
        //        string strContent = sr.ReadToEnd();
        //        responseStream.Close();
        //        resp.Close();

        //        Deployment.Current.Dispatcher.BeginInvoke(() =>
        //        {
        //            _rs.IsConnect = (strContent.Contains("Mon bureau iCampus"));
        //        });
        //    }
        //    catch (WebException)
        //    {
        //        req.Abort();
        //    }
        //}

        public void SyncSite()
        {
            AppSettings set = new AppSettings();
            string url = set.MainSiteSetting + "/claroline/auth/login.php?login=" + set.UsernameSetting + "&password=" + set.PasswordSetting + "&sourceUrl=aHR0cDovL2ljYW1wdXMudWNsb3V2YWluLmJlL2luZGV4LnBocA%3D%3D";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            // Appel asynchrone
            try
            {
                IAsyncResult Async = req.BeginGetResponse(new AsyncCallback(ResponseSyncCallback), req);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResponseSyncCallback(IAsyncResult ar)
        {
            //Récupération de la requete web (object HttpWebRequest)
            HttpWebRequest req = (HttpWebRequest)ar.AsyncState;
            //Récupération de la réponse Web
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);
                Stream responseStream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1"));
                string strContent = sr.ReadToEnd();
                responseStream.Close();
                cookies.Add(new Uri((new AppSettings()).MainSiteSetting),resp.Cookies);
                resp.Close();

//                Debug.WriteLine(strContent.Contains("WARNING")?strContent:strContent.Substring(0, 50));

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (App.currentProgressInd != null)
                    {
                        App.currentProgressInd.Text = "Ajout en DB...";
                        App.currentProgressInd.IsVisible = true;
                    }
                    (new Update(true)).readHTML(strContent);

                    if (App.currentProgressInd != null)
                    {
                        App.currentProgressInd.Text = "";
                        App.currentProgressInd.IsVisible = false;
                    }
                });
            }
            catch (WebException)
            {
                req.Abort();
            }
        }

        public void GETPage(string url)
        {
            if (String.IsNullOrEmpty(url)) url = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create((new AppSettings()).MainSiteSetting + url);
            // Création de l'objet état
            //On ajoute la requete dans l'objet état pour pouvoir le récupérer dans la callback
            req.Method = "GET";
            req.CookieContainer = cookies;
            //            Debug.WriteLine(url);
            if (App.currentProgressInd != null)
            {
                App.currentProgressInd.Text = "Requête iCampus...";
                App.currentProgressInd.IsVisible = true;
            }
            try
            {
                IAsyncResult Async = req.BeginGetResponse(new AsyncCallback(ResponseSyncCallback), req);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
