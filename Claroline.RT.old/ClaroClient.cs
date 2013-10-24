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
using Newtonsoft.Json;
using System.Threading.Tasks;
using ClarolineApp.RT.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using ClarolineApp.RT.Models;

namespace ClarolineApp.RT
{
    public class ClarolineClient : INotifyPropertyChanged
    {

        private static ClarolineClient _Current;
        public static ClarolineClient Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new ClarolineClient();
                }
                return _Current;
            }
        }

        ApplicationModel settings = ApplicationModel.Current;

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
#if DEBUG
                    Debug.WriteLine("Exception occured :" + value.ToString());
#endif
                    _lastException = value;
                    NotifyPropertyChanged("lastException");
                }
            }
        }

        public ClarolineClient()
        {
            //WebRequesting stuff
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
        }

        private async Task<string> getResponseAsync(PostDataWriter args)
        {
            //Data request
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = false;
            handler.UseCookies = true;
            HttpClient client = new HttpClient(handler);

            HttpResponseMessage response = null;
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, args.GetURL());

#if DEBUG
            Debug.WriteLine(args.GetURL() + " / " + args.GetPostDataString());
#endif

            StringContent content = new StringContent(args.GetPostDataString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            message.Content = content;
            message.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("gzip"));
            response = await client.SendAsync(message);
            string resp;
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                resp = String.Empty;
            }
            else
            {
                resp = await response.Content.ReadAsStringAsync();
            }
            return resp;
        }

        public async Task<string> makeOperationAsync(SupportedModules module, SupportedMethods method, Cours reqCours = null, string resStr = "", bool forAuth = false)
        {
            if (IsNetworkAvailable() && (forAuth || await isValidAccountAsync(forAuth)))
            {
                PostDataWriter args = new PostDataWriter() { module = module, method = method, cidReq = reqCours, resStr = resStr };

                String strContent = "";
                try
                {

                    strContent = await getResponseAsync(args);
#if DEBUG
                    Debug.WriteLine("Call for :" + module + "/" + method + "\nResponse :" + strContent + "\n");
#endif
                }
                catch (Exception ex)
                {
                    lastException = ex;
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
            String result = await makeOperationAsync(SupportedModules.NOMOD, SupportedMethods.authenticate, forAuth: true);
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
            return true;//(DeviceNetworkInformation.IsNetworkAvailable && (DeviceNetworkInformation.IsWiFiEnabled || (DeviceNetworkInformation.IsCellularDataEnabled && settings.CellularDataEnabledSetting)));
#else
            return ApplicationModel.Current.IsConnected;
#endif
        }

        public async Task<Boolean> isValidAccountAsync(bool forAuth = false)
        {
            if (!forAuth && isExpired && IsNetworkAvailable())
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

        public void invalidateClient()
        {
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
            settings.Login = "";
            settings.Password = "";
            //settings.U = new User();
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

        class NetworkException : WebException
        {
            public NetworkException(string message)
                : base(message)
            { }
        }
        class AuthenticationException : WebException
        {
            public AuthenticationException(string message)
                : base(message)
            { }
        }

    }

}
