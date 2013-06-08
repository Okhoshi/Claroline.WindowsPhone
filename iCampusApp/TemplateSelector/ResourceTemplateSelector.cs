using ClarolineApp.Model;
using ClarolineApp.Settings;
using System.Windows;

namespace ClarolineApp.TemplateSelector
{
    public class ResourceTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Annonce
        {
            get;
            set;
        }

        public DataTemplate Document
        {
            get;
            set;
        }

        public DataTemplate Description
        {
            get;
            set;
        }

        public DataTemplate Event
        {
            get;
            set;
        }

        public DataTemplate Generic
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ResourceModel rm = item as ResourceModel;

            if (rm != null)
            {
                switch (rm.DiscKey)
                {
                    case SupportedModules.CLDOC:
                        return Document;
                    case SupportedModules.CLANN:
                        return Annonce;
                    case SupportedModules.CLDSC:
                        return Description;
                    case SupportedModules.CLCAL:
                        return Event;
                    default:
                        return Generic;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
