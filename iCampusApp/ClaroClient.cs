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

        private bool _inSync;

        public bool IsInSync
        {
            get
            {
                return _inSync;
            }
            private set
            {
                if (!_inSync == value)
                {
                    _inSync = value;
                    NotifyPropertyChanged("IsInSync");
                }
            }
        }

        public bool isExpired
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
                if (!value.Equals(_lastException))
                {
                    _lastException = value;
#if DEBUG
                        Debug.WriteLine("Exception occured :" + value.ToString());
#endif
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

        public async Task<string> makeOperationAsync(SupportedModules module, SupportedMethods method, Cours reqCours = null, string resStr = "", bool forAuth = false)
        {
            _lastException = null;

            if (IsNetworkAvailable() && ( forAuth || await isValidAccountAsync()))
            {
                PostDataWriter args = new PostDataWriter() { module = module, method = method, cidReq = reqCours, resStr = resStr };

                String strContent = "";
                HttpWebResponse response = null;
                Stream responseStream = null;
                try
                {
                    IsInSync = true;
                    HttpWebRequest client = await getClientAsync(args);
                    response = (HttpWebResponse)await client.GetResponseAsync();
                    responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                    strContent = sr.ReadToEnd();
                    responseStream.Close();
                    response.Close();

#if DEBUG
                    Debug.WriteLine("Call for :" + module + "/" + method + "\nResponse :" + strContent);
#endif
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

                IsInSync = false;

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
            String result = await makeOperationAsync(SupportedModules.NOMOD, SupportedMethods.authenticate, forAuth:true);
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
