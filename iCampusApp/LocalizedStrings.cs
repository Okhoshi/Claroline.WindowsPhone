using ClarolineApp.Languages;

namespace ClarolineApp
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static AppLanguage localizedResources = new ClarolineApp.Languages.AppLanguage();

        public AppLanguage LocalizedResources { get { return localizedResources; } }
    }

}
