using ClarolineApp.Settings;
using ClarolineApp.VM;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClarolineApp
{
    public class ClarolineClient : INotifyPropertyChanged
    {
        public ISettings Settings;

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

        public Exception LastException
        {
            get { return _lastException; }
            set
            {
                if (_lastException == null || !_lastException.Equals(value))
                {
#if DEBUG
                    if (value != null)
                    {
                        Debug.WriteLine("Exception occured :" + value.ToString());
                    }
#endif
                    _lastException = value;
                    NotifyPropertyChanged("LastException");
                }
            }
        }

        private bool _IsInSync;

        public bool IsInSync
        {
            get
            {
                return _IsInSync;
            }
            set
            {
                if (value != _IsInSync)
                {
                    _IsInSync = value;
                    NotifyPropertyChanged("IsInSync");
                }
            }
        }

        public ClarolineClient(ISettings settings)
        {
            Settings = settings;

            //WebRequesting stuff
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
        }

        private async Task<string> GetResponseAsync(PostDataWriter args)
        {
            //Data request
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = args.method == SupportedMethods.GetPage;
            handler.UseCookies = true;
            HttpClient client = new HttpClient(handler);

            HttpResponseMessage response = null;
            HttpRequestMessage message = new HttpRequestMessage(args.method == SupportedMethods.GetPage ? HttpMethod.Get : HttpMethod.Post, args.GetUrl());

#if DEBUG
            Debug.WriteLine(args.GetUrl() + " / " + args.GetPostDataString());
#endif
            if (args.method != SupportedMethods.GetPage)
            {
            StringContent content = new StringContent(args.GetPostDataString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            message.Content = content;
        }
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

        public async Task<string> MakeOperationAsync(SupportedModules module, SupportedMethods method, string syscode = "", string resStr = "", string genMod = "", bool forAuth = false)
        {
            if (IsNetworkAvailable() && (forAuth || await IsValidAccountAsync(forAuth)))
            {
                PostDataWriter args = new PostDataWriter() { module = module, method = method, cidReq = syscode, resStr = resStr, GenMod = genMod };

                String strContent = "";
                try
                {

                    strContent = await GetResponseAsync(args);
#if DEBUG
                    Debug.WriteLine("Call for :" + module + "/" + method + "\nResponse :" + strContent + "\n");
#endif
                    if (args.method != SupportedMethods.GetPage && strContent.StartsWith("<"))
                    {
                        strContent = "";
                    }
                }
                catch (Exception ex)
                {
                    LastException = ex;
                }
                return strContent;
            }
            else
            {
                LastException = new NetworkException("Network Unavailable");
                return null;
            }
        }

        private async Task<bool> GetSessionCookieAsync()
        {
            String result = await MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.Authenticate, forAuth: true);
            if (result.Equals(String.Empty))
            {
                cookieCreation = DateTime.Now;
                return true;
            }
            else
            {
                LastException = new AuthenticationException("Authentication Fails");
                return false;
            }
        }

        public static bool IsNetworkAvailable()
        {
#if !DEBUG
            return true;//(DeviceNetworkInformation.IsNetworkAvailable && (DeviceNetworkInformation.IsWiFiEnabled || (DeviceNetworkInformation.IsCellularDataEnabled && settings.CellularDataEnabledSetting)));
#else
#if !WINDOWS_PHONE
            return ApplicationModel.Current.IsConnected;
#else
            return ViewModelLocator.Client.Settings.CellularDataEnabledSetting;
#endif
#endif
        }

        public async Task<Boolean> IsValidAccountAsync(bool forAuth = false)
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

        public Boolean IsValidAccountSync()
        {
            return !isExpired;
        }

        public void InvalidateClient()
        {
            cookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
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
