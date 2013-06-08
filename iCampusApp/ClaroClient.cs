using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if !DEBUG
    using Microsoft.Phone.Net.NetworkInformation;
#endif

namespace ClarolineApp
{
    public class ClaroClient : INotifyPropertyChanged
    {

        private static ClaroClient _instance;
        public static ClaroClient Instance
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

        public static AppSettings Settings
        {
            get
            {
                return AppSettings.Instance;
            }
        }

        CookieContainer Cookies;
        
        private DateTime _cookieCreation;
        
        DateTime CookieCreation
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
                    NotifyPropertyChanged("CookieCreation");
                    NotifyPropertyChanged("IsExpired");
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

        public bool IsExpired
        {
            get
            {
                return (CookieCreation.AddHours(1.0).CompareTo(DateTime.Now) < 0);
            }
        }

        private Exception _lastException;

        public Exception LastException
        {
            get { return _lastException; }
            set
            {
                if (value != null && !value.Equals(_lastException))
                {
                    _lastException = value;
#if DEBUG
                    Debug.WriteLine("Exception occured :" + value.ToString());
#endif
                    NotifyPropertyChanged("LastException");
                }
            }
        }

        public ClaroClient()
        {
            //WebRequesting stuff
            CookieCreation = DateTime.MinValue;
            Cookies = new CookieContainer();
        }

        private async Task<HttpWebRequest> GetClientAsync(PostDataWriter args)
        {
            //Data request
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(args.GetUrl());
            client.CookieContainer = Cookies;
            client.AllowAutoRedirect = args.method == SupportedMethods.GetPage;
            client.Method = args.method == SupportedMethods.GetPage ? "GET" : "POST";

            if (args.method != SupportedMethods.GetPage)
            {
                client.ContentType = "application/x-www-form-urlencoded";

                Stream postStream = await client.GetRequestStreamAsync();
                String postData = args.GetPostDataString();

                // Convert the string into a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Write to the request stream.
                postStream.Write(byteArray, 0, postData.Length);
                postStream.Close();
            }

            return client;
        }

        public async Task<string> MakeOperationAsync(SupportedModules module, SupportedMethods method, Cours reqCours = null, string resStr = "", string genMod = "", bool forAuth = false)
        {
            _lastException = null;

            if (IsNetworkAvailable() && ( forAuth || await IsValidAccountAsync()))
            {
                PostDataWriter args = new PostDataWriter() { module = module, method = method, cidReq = reqCours, resStr = resStr, GenMod = genMod };

                String strContent = "";
                HttpWebResponse response = null;
                Stream responseStream = null;
                try
                {
                    IsInSync = true;
                    HttpWebRequest client = await GetClientAsync(args);
                    response = (HttpWebResponse)await client.GetResponseAsync();
                    strContent = DecodeData(response);

                    if (forAuth)
                    {
                        foreach (string cookie in response.Headers["Set-Cookie"].Split(new string[] { ",  " }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            int indexOfEqual = cookie.IndexOf("=");
                            Cookies.Add(new Uri(Settings.DomainSetting, UriKind.Absolute), new Cookie(cookie.Substring(0, indexOfEqual), cookie.Substring(indexOfEqual + 1, cookie.IndexOf(";") - (indexOfEqual + 1))));
                        }
                    }
                    response.Close();

#if DEBUG
                    Debug.WriteLine("Call for :" + module + "/" + method + "\nResponse :" + strContent);
#endif
                }
                catch (Exception ex)
                {
                    LastException = ex;

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
                LastException = new NetworkException("Network Unavailable");
                return null;
            }
        }

        private async Task<bool> GetSessionCookieAsync()
        {
            String result = await MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.Authenticate, forAuth:true);
            if (result.Equals(String.Empty))
            {
                CookieCreation = DateTime.Now;
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
            return (DeviceNetworkInformation.IsNetworkAvailable && (DeviceNetworkInformation.IsWiFiEnabled || (DeviceNetworkInformation.IsCellularDataEnabled && Settings.CellularDataEnabledSetting)));
#else
            return Settings.CellularDataEnabledSetting;
#endif
        }

        public async Task<Boolean> IsValidAccountAsync()
        {
            if (IsExpired && IsNetworkAvailable())
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
            return !IsExpired;
        }

        public void InvalidateClient()
        {
            CookieCreation = DateTime.MinValue;
            Cookies = new CookieContainer();
        }

        private static string DecodeData(WebResponse w)
        {

            //
            // first see if content length header has charset = calue
            //
            String charset = null;
            String ctype = w.Headers["content-type"];
            if (ctype != null)
            {
                int ind = ctype.IndexOf("charset=");
                if (ind != -1)
                {
                    charset = ctype.Substring(ind + 8);
                    Console.WriteLine("CT: charset=" + charset);
                }
            }

            // save data to a memorystream
            MemoryStream rawdata = new MemoryStream();
            byte[] buffer = new byte[1024];
            Stream rs = w.GetResponseStream();
            int read = rs.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                rawdata.Write(buffer, 0, read);
                read = rs.Read(buffer, 0, buffer.Length);
            }

            rs.Close();

            //
            // if ContentType is null, or did not contain charset, we search in body
            //
            if (charset == null)
            {
                MemoryStream ms = rawdata;
                ms.Seek(0, SeekOrigin.Begin);

                StreamReader srr = new StreamReader(ms, Encoding.UTF8);
                String meta = srr.ReadToEnd();

                if (meta != null)
                {
                    int start_ind = meta.IndexOf("charset=");
                    int end_ind = -1;
                    if (start_ind != -1)
                    {
                        end_ind = meta.IndexOf("\"", start_ind);
                        if (end_ind != -1)
                        {
                            int start = start_ind + 8;
                            charset = meta.Substring(start, end_ind - start + 1);
                            charset = charset.TrimEnd(new Char[] { '>', '"' });
                            Console.WriteLine("META: charset=" + charset);
                        }
                    }
                }
            }

            Encoding e = null;
            if (charset == null)
            {
                e = Encoding.UTF8; //default encoding
            }
            else
            {
                try
                {
                    e = Encoding.GetEncoding(charset);
                }
                catch (Exception ee)
                {
                    Console.WriteLine("Exception: GetEncoding: " + charset);
                    Console.WriteLine(ee.ToString());
                    e = Encoding.UTF8;
                }
            }

            rawdata.Seek(0, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(rawdata, e);

            String s = sr.ReadToEnd();

            return s;
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
