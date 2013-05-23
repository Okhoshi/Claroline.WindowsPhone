using ClarolineApp.Model;
using System.Windows;

namespace ClarolineApp.TemplateSelector
{
    public class ResourceListTemplateSelector : DataTemplateSelector
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

        public DataTemplate Notification
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
            ResourceList list = item as ResourceList;

            if (list != null)
            {
                switch (list.label)
                {
                    case CL_Document.LABEL:
                        return Document;
                    case CL_Annonce.LABEL:
                        return Annonce;
                    case CL_Description.LABEL:
                        return Description;
                    case CL_Event.LABEL:
                        return Event;
                    default:
                        return Generic;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
