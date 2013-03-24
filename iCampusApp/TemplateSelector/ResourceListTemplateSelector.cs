using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
        public DataTemplate Notification
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
                if (list.ressourceType == typeof(CL_Annonce))
                {
                    return Annonce;
                }
                else if (list.ressourceType == typeof(CL_Document))
                {
                    return Document;
                }
                else if (list.ressourceType == typeof(CL_Notification))
                {
                    return Notification;
                }
                else
                {
                    return Generic;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
