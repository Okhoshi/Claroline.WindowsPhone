using System;
using System.Collections.Generic;
using System.Linq;
using ClarolineApp.RT.Models;
using System.Text;

namespace ClarolineApp.RT
{
    public enum SupportedModules
    {
        NOMOD,
        USER,
        CLDOC,
        CLANN
    }

    public enum SupportedMethods
    {
        //NOMOD
        authenticate,
        //USER
        getUserData,
        getCourseList,
        getCourseToolList,
        getUpdates,
        //CLXXX
        getResourcesList,
        getSingleResource
    }

    public class PostDataWriter
    {
        public static String login = ApplicationModel.Current.Login;
        public static String password = ApplicationModel.Current.Password;
        public SupportedModules module;
        public SupportedMethods method;
        public Cours cidReq;
        public string resStr;

        private static string GetEnumName(Enum enumeration)
        {
            return Enum.GetName(enumeration.GetType(), enumeration);
        }

        public string GetURL()
        {
            string URL = "";

            switch (module)
            {
                case SupportedModules.NOMOD:
                    URL = ApplicationModel.Current.Domain + "/claroline/auth/login.php";
                    break;
                case SupportedModules.USER:
                case SupportedModules.CLANN:
                case SupportedModules.CLDOC:
                    URL = ApplicationModel.Current.Domain + ApplicationModel.Current.WebServicePath;
                    break;
                default:
                    throw new NotSupportedException(GetEnumName(module) + " is not supported");
            }

            return URL;
        }

        public string GetPostDataString()
        {
            string postString = "";

            switch (module)
            {
                case SupportedModules.NOMOD:
                    switch (method)
                    {
                        case SupportedMethods.authenticate:
                            postString = "login=" + login + "&password=" + password;
                            break;
                        default:
                            throw new NotSupportedException(
                                        Enum.GetName(typeof(SupportedMethods), method)
                                        + " is not supported in "
                                        + Enum.GetName(typeof(SupportedModules), module));
                    }
                    break;
                case SupportedModules.USER:
                    switch (method)
                    {
                        case SupportedMethods.getCourseToolList:
                            if (cidReq != null)
                            {
                                postString = "cidReset&cidReq=" + cidReq.sysCode + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            }
                            else
                            {
                                throw new ArgumentNullException("cidReq cannot be null");
                            }
                            break;
                        case SupportedMethods.getUpdates:
                        case SupportedMethods.getCourseList:
                        case SupportedMethods.getUserData:
                            postString += "module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            break;
                        default:
                            throw new NotSupportedException(
                                        GetEnumName(method)
                                        + " is not supported in "
                                        + GetEnumName(module));
                    }
                    break;
                case SupportedModules.CLANN:
                case SupportedModules.CLDOC:
                    switch (method)
                    {
                        case SupportedMethods.getResourcesList:
                            if (cidReq != null)
                            {
                                postString = "cidReset&cidReq=" + cidReq.sysCode + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            }
                            else
                            {
                                throw new ArgumentNullException("cidReq cannot be null");
                            }
                            break;
                        case SupportedMethods.getSingleResource:
                            if (cidReq != null && resStr != null)
                            {
                                postString = "cidReset&cidReq=" + cidReq.sysCode + "&resID=" + resStr + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            }
                            else
                            {
                                if (cidReq == null)
                                {
                                    throw new ArgumentNullException("cidReq cannot be null");
                                }
                                else if (resStr == null)
                                {
                                    throw new ArgumentOutOfRangeException("resStr must be setted");
                                }
                            }
                            break;
                        default:
                            throw new NotSupportedException(
                                        GetEnumName(method)
                                        + " is not supported in "
                                        + GetEnumName(module));
                    }
                    break;
                default:
                    throw new NotSupportedException(GetEnumName(module) + " is not supported");
            }

            return postString;
        }
    }
}