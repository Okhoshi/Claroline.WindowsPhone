using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp
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
        getToolList,
        getUpdates,
        //CLXXX
        getResourcesList,
        getSingleResource
    }

    public class PostDataWriter
    {
        public static String login = AppSettings.instance.UsernameSetting;
        public static String password = AppSettings.instance.PasswordSetting;
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
                    URL = AppSettings.instance.DomainSetting + AppSettings.instance.AuthPageSetting;
                    break;
                case SupportedModules.USER:
                case SupportedModules.CLANN:
                case SupportedModules.CLDOC:
                    URL = AppSettings.instance.DomainSetting + AppSettings.instance.WebServiceSetting;
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
                        case SupportedMethods.getToolList:
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