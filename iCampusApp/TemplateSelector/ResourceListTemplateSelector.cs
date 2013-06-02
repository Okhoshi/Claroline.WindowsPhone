using ClarolineApp.Model;
using ClarolineApp.Settings;
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

        public DataTemplate Menu
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
                    case Model.Document.Label:
                        return Document;
                    case Model.Annonce.Label:
                        return Annonce;
                    case Model.Description.Label:
                        return Description;
                    case Model.Event.Label:
                        return Event;
                    case "MENU":
                        return Menu;
                    default:
                        return Generic;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
