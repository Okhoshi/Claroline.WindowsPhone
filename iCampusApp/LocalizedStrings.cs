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

namespace ClarolineApp
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static ClarolineApp.Languages.AppLanguage localizedResources = new ClarolineApp.Languages.AppLanguage();

        public ClarolineApp.Languages.AppLanguage LocalizedResources { get { return localizedResources; } }
    }

}
