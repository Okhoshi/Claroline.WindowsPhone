using ClarolineApp.Model;
using ClarolineApp.Settings;
using System.Windows;

namespace ClarolineApp.TemplateSelector
{
    public class ResourceTemplateSelector : ResourceListTemplateSelector
    {
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
                    case SupportedModules.CLFRM:
                        return Forum;
                    default:
                        return Generic;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
