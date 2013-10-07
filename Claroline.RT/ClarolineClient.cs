using ClarolineOnTablet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineOnTablet
{
    public class ClarolineClient : ViewModelBase
    {
        private static ClarolineClient instance;

        public static ClarolineClient Current
        {
            get
            {
                if (instance == null)
                    instance = new ClarolineClient();
                return instance;
            }
        }

        public static CookieContainer cookies;
        private static DateTime CookieCreation;
        public static Boolean isExpired
        {
            get { return (CookieCreation.AddHours(1.0).CompareTo(DateTime.Now) < 0); }
        }


        public ClarolineClient()
        {
            //WebRequesting stuff
            CookieCreation = DateTime.MinValue;
            cookies = new CookieContainer();
        }

        private HttpClient getClient()
        {
            //Data request
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = false;
            handler.UseCookies = true;
            HttpClient client = new HttpClient(handler);
            return client;
        }

        public async Task<bool> getSessionCookieAsync()
        {
            Debug.WriteLine("I'm here");
            HttpClient request = null;
            HttpResponseMessage response = null;
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post,
                                         ApplicationModel.Current.Domain + "/claroline/auth/login.php");

            Debug.WriteLine("Message : " + message.ToString());
            CallbackArgs args = new CallbackArgs() { login = ApplicationModel.Current.Login, password = ApplicationModel.Current.Password, operation = AllowedOperations.authenticate };
            StringContent content = new StringContent(args.ToString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            message.Content = content;

            request = getClient();
            response = await request.SendAsync(message);

            Debug.WriteLine("getSessionCookie : " + response.StatusCode);

            bool empty = String.IsNullOrEmpty(await response.Content.ReadAsStringAsync());
            if (empty)
            {
                CookieCreation = DateTime.Now;
            }
            Debug.WriteLine("empty : " + empty.ToString());
            return empty;
        }


        public async Task<string> makeOperationAsync(AllowedOperations op, Cours reqCours = null, int resID = -1)
        {
            if (ApplicationModel.Current.IsConnected)
            {
                if (!isExpired || (await getSessionCookieAsync()))
                {
                    CallbackArgs args = null;
                    switch (op)
                    {
                        case AllowedOperations.getSingleAnnounce:
                            if (reqCours == null && resID < 0)
                            {
                                return "";
                            }
                            args = new CallbackArgs() { cidReq = reqCours, resId = resID, operation = op };
                            break;
                        case AllowedOperations.getCourseToolList:
                        case AllowedOperations.getDocList:
                        case AllowedOperations.getAnnounceList:
                            if (reqCours == null)
                            {
                                return "";
                            }
                            args = new CallbackArgs() { cidReq = reqCours, operation = op };
                            break;
                        case AllowedOperations.getUserData:
                        case AllowedOperations.getCourseList:
                        case AllowedOperations.getUpdates:
                            args = new CallbackArgs() { operation = op };
                            break;
                    }

                    HttpResponseMessage response = null;
                    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post,
                                                 ApplicationModel.Current.Domain + ApplicationModel.Current.WebServicePath);
                    
                    StringContent content = new StringContent(args.ToString());
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    message.Content = content;
                    HttpClient request = getClient();
                    response = await request.SendAsync(message);
                    String result = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("Fin de la méthode " + op.ToString() + " : " + result);
                    return result;
                }
            }
                //TODO tell the user the network is unavailable
                return "";

        }

    }

    public class CallbackArgs
    {
        public AllowedOperations operation;
        public Cours cidReq;
        public String login;
        public String password;
        public int resId;

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
                case AllowedOperations.getSingleAnnounce:
                    return "Method=getSingleAnnounce&cidReq=" + cidReq.sysCode + "&resId=" + resId;
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
        getSingleAnnounce,
        getUpdates
    }

}
