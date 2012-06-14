using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.IO;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace PhoneApp1
{
    public class ClaroClientTest
    {
        CookieContainer cookies;
        DateTime CookieExpiration;
        HttpWebRequest client;
        HttpWebRequest auth;
        Mutex Requesting;
        BackgroundWorker bw;

        public ClaroClientTest()
        {
            Requesting = new Mutex(false, "Request");
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = false;
            bw.DoWork += new DoWorkEventHandler(DoWork);

            CookieExpiration = DateTime.MinValue;

            cookies = new CookieContainer();
            client = (HttpWebRequest) WebRequest.Create("http://ipv4.fiddler/claroline/module/MOBILE/index.php");
            client.Method = "POST";
            client.CookieContainer = cookies;
            client.ContentType = "application/x-www-form-urlencoded";
            client.AllowAutoRedirect = false;

            auth = (HttpWebRequest)WebRequest.Create("http://ipv4.fiddler/Claroline/claroline/auth/login.php");
            auth.Method = "POST";
            auth.CookieContainer = cookies;
            auth.ContentType = "application/x-www-form-urlencoded";
            auth.AllowAutoRedirect = false;
        }

        public void testgetCourseList()
        {
        }

        public void testGetUserId()
        {
            bw.RunWorkerAsync();
        }

        public void testGetDocList()
        {

        }

        private void getSessionCookie()
        {
            CallbackArgs args = new CallbackArgs() { req = auth, postdata = "login=admin&password=elegie24", operation = AllowedOperations.authenticate };
            setProgressIndicator(true, "Connexion...");
            auth.BeginGetRequestStream(new AsyncCallback(RequestCallback), args);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (isExpired())
            {
                getSessionCookie();

                lock (Requesting)
                {
                    Debug.WriteLine("Wait Pulse");
                    Monitor.Wait(Requesting);
                }
            }

            CallbackArgs args = new CallbackArgs() { req = client, postdata = "Method=getUserID", operation = AllowedOperations.getUserID };
            client.BeginGetRequestStream(new AsyncCallback(RequestCallback), args); ;
        }

        private void RequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = ((CallbackArgs) ar.AsyncState).req;
            String postData = ((CallbackArgs)ar.AsyncState).postdata;
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
            //Récupération de la requete web (object HttpWebRequest)
            HttpWebRequest Request = ((CallbackArgs)ar.AsyncState).req;
            AllowedOperations op = ((CallbackArgs)ar.AsyncState).operation;
            //Récupération de la réponse Web
            try
            {
                HttpWebResponse Response = (HttpWebResponse)Request.EndGetResponse(ar);
                Stream responseStream = Response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                string strContent = sr.ReadToEnd();
                responseStream.Close();
                Response.Close();

                switch (op)
                {
                    case AllowedOperations.getUserID:
                        Debug.WriteLine(strContent);
                        break;
                    case AllowedOperations.authenticate:
                        if (!strContent.Contains("<div id=\"loginBox\">"))
                        {
                            CookieExpiration = DateTime.Now;
                        }
                        else
                        {
                            MessageBox.Show("La connexion a échoué");
                        }
                        pulse();
                        break;
                    default:
                        break;
                }

                setProgressIndicator(false);
            }
            catch (WebException ex)
            {
                Request.Abort();
                Debug.WriteLine(ex.Message);
            }
        }

        private void pulse()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                lock (Requesting)
                {
                    Monitor.PulseAll(Requesting);
                    Debug.WriteLine("PULSING");
                }
            });
        }

        public bool isExpired()
        {
            return (CookieExpiration.AddHours(1.0).CompareTo(DateTime.Now) < 0);
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
    }

    public class CallbackArgs
	{
		public HttpWebRequest req;
        public String postdata = "";
        public AllowedOperations operation;
	}

    public enum AllowedOperations
    {
        authenticate,
        getUserID,
        getCourseList,
        getCourseToolList,
        getDocList,
        getAnnounceList
    }
}
