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
using System.Threading;

namespace iCampusApp
{
    public class ConnectClient
    {
        private WebClient client;
        private int ThreadId;
        private bool loggedIn;

        public ConnectClient()
        {
            client = new WebClient();
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
            client.DownloadStringCompleted +=new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            loggedIn = false;
        }

        public void logOn(string url, string username, string password)
        {            
            try
            {
                string data = "login="+username+"&password="+password;
                // actually execute the POST request
                client.UploadStringAsync(new Uri(url,UriKind.RelativeOrAbsolute),"POST", data);

                Thread.CurrentThread.Join();
                if (loggedIn)
                {
                getPage(url);
                }
            }
            catch (WebException we)
            {
                // WebException.Status holds useful information
                Console.WriteLine(we.Message + "\n" + we.Status.ToString());
            }
            catch (NotSupportedException ne)
            {
                // other errors
                Console.WriteLine(ne.Message);
            }
        }

        public void getPage(string url)
        {
            client.DownloadStringAsync(new Uri(url));
        }

        public void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            loggedIn = e.Result != null && e.Result.Contains(@"<div id=""userDetails""><p><span>Utilisateur</span><br />");
        }

        public void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            (new Update(true)).readHTML(e.Result);
        }
    }
}
