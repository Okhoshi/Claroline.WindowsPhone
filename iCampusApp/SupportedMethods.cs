using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;

namespace ClarolineApp
{
    public enum SupportedModules
    {
        NOMOD,
        USER,
        GENERIC,
        CLDOC,
        CLANN,
        CLDSC,
        CLCAL,
        CLFRM
    }

    public enum SupportedMethods
    {
        //NOMOD
        Authenticate,
        GetPage,
        //USER
        GetUserData,
        GetCourseList,
        GetToolList,
        GetUpdates,
        //CLXXX
        GetResourcesList,
        GetSingleResource
    }

    public class PostDataWriter
    {
        public String login = AppSettings.Instance.UserNameSetting;
        public String password = AppSettings.Instance.PasswordSetting;
        public SupportedModules module;
        public SupportedMethods method;
        public string GenMod;
        public Cours cidReq;
        public string resStr;

        private static string GetEnumName(Enum enumeration)
        {
            return Enum.GetName(enumeration.GetType(), enumeration);
        }

        public string GetUrl()
        {
            string URL = "";

            switch (module)
            {
                case SupportedModules.NOMOD:
                    switch (method)
                    {
                        case SupportedMethods.Authenticate:
                            URL = AppSettings.Instance.DomainSetting + AppSettings.Instance.AuthPageSetting;
                            break;
                        case SupportedMethods.GetPage:
                            URL = GenMod;
                            break;
                        default:
                            throw new NotSupportedException(
                                        GetEnumName(method)
                                        + " is not supported in "
                                        + GetEnumName(module));
                    }
                    break;
                default:
                    if (Enum.IsDefined(typeof(SupportedModules), module))
                    {
                        URL = AppSettings.Instance.DomainSetting + AppSettings.Instance.WebServiceSetting;
                        break;
                    }
                    else
                    {
                        throw new NotSupportedException(GetEnumName(module) + " is not supported");
                    }
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
                        case SupportedMethods.Authenticate:
                            postString = "login=" + login + "&password=" + password;
                            break;
                        case SupportedMethods.GetPage:
                            postString = "";
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
                        case SupportedMethods.GetToolList:
                            if (cidReq != null)
                            {
                                postString = "cidReset&cidReq=" + cidReq.sysCode + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            }
                            else
                            {
                                throw new ArgumentNullException("cidReq");
                            }
                            break;
                        case SupportedMethods.GetUpdates:
                        case SupportedMethods.GetCourseList:
                        case SupportedMethods.GetUserData:
                            postString += "module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                            break;
                        default:
                            throw new NotSupportedException(
                                        GetEnumName(method)
                                        + " is not supported in "
                                        + GetEnumName(module));
                    }
                    break;
                case SupportedModules.GENERIC:
                    switch (method)
                    {
                        case SupportedMethods.GetResourcesList:
                            if (cidReq != null && GenMod != null)
                            {
                                postString = "cidReset&cidReq=" + cidReq.sysCode + "&module=" + GenMod + "&method=" + GetEnumName(method);
                            }
                            else
                            {
                                if (cidReq == null)
                                {
                                    throw new ArgumentNullException("cidReq");
                                }
                                else if (GenMod == null)
                                {
                                    throw new ArgumentOutOfRangeException("genMod");
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
                    if (Enum.IsDefined(typeof(SupportedModules), module))
                    {
                        switch (method)
                        {
                            case SupportedMethods.GetResourcesList:
                                if (cidReq != null)
                                {
                                    postString = "cidReset&cidReq=" + cidReq.sysCode + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                                }
                                else
                                {
                                    throw new ArgumentNullException("cidReq");
                                }
                                break;
                            case SupportedMethods.GetSingleResource:
                                if (cidReq != null && resStr != null)
                                {
                                    postString = "platform=WP&cidReset&cidReq=" + cidReq.sysCode + "&resID=" + resStr + "&module=" + GetEnumName(module) + "&method=" + GetEnumName(method);
                                }
                                else
                                {
                                    if (cidReq == null)
                                    {
                                        throw new ArgumentNullException("cidReq");
                                    }
                                    else if (resStr == null)
                                    {
                                        throw new ArgumentOutOfRangeException("resStr");
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
                    }
                    else
                    {
                        throw new NotSupportedException(GetEnumName(module) + " is not supported");
                    }
            }

            return postString;
        }
    }
}