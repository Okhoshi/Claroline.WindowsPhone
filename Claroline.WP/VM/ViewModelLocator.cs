/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Claroline.RT"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using ClarolineApp.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ClarolineApp.VM
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ISettings, AppSettings>();

            SimpleIoc.Default.Register<ClarolineClient>();

            SimpleIoc.Default.Register<ClarolineVM>();
            SimpleIoc.Default.Register<MainPageVM>();
        }

        public static ClarolineClient Client
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ClarolineClient>();
            }
        }

        public ClarolineVM ClarolineVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ClarolineVM>();
            }
        }

        public MainPageVM MainPageVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageVM>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}