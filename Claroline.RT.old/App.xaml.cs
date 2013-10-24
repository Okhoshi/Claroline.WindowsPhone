using Callisto.Controls;
using ClarolineApp.RT.Common;
using ClarolineApp.RT.Controls;
using ClarolineApp.RT.Models;
using ClarolineApp.RT.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle Application grille, consultez la page http://go.microsoft.com/fwlink/?LinkId=234226

namespace ClarolineApp.RT
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {

        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>
        /// The root frame.
        /// </value>
        public Frame RootFrame
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public CoreDispatcher CoreDispatcher
        {
            get;
            private set;
        }

        public void UpdateTileNotifications()
        {
            //
            // get update
            //
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
            //
            // clear
            //
            updater.Clear();
            //
            // Update up to 5 notifications
            //
            updater.EnableNotificationQueue(true);
            //
            // loop 
            //
            foreach (Cours item in ApplicationModel.Current.AllCours)
            {
                //
                // get the template
                //
                var xmlTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText09);
                var tn1 = xmlTemplate.SelectSingleNode("/tile/visual/binding/text[@id='1']");
                var nt1 = xmlTemplate.CreateTextNode(item.title);
                tn1.AppendChild(nt1);
                var tn2 = xmlTemplate.SelectSingleNode("/tile/visual/binding/text[@id='2']");
                var nt2 = xmlTemplate.CreateTextNode(item.title);
                tn2.AppendChild(nt2);
                //
                // create notification
                //
                var tileNotification = new TileNotification(xmlTemplate);
                //
                // update
                //
                updater.Update(tileNotification);
            }
        }


        private async void EnsureInitialized(Frame frame, IActivatedEventArgs args)
        {
            //
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            //
            // create application
            //
            ApplicationModel.Current.AllCours.CollectionChanged += (sender, ncce) =>
            {
                //
                // update tiles
                //
                UpdateTileNotifications();
            };
            //
            // hook up
            //
            SearchPane.GetForCurrentView().QuerySubmitted += App_QuerySubmitted;
            //
            // set frame
            //
            RootFrame = frame;
            //
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            //
            if (RootFrame == null)
            {
                //
                // Create a Frame to act as the navigation context and navigate to the first page
                //
                RootFrame = new CharmFrame { CharmContent = new Settings.Settings() };
                //
                //Associate the frame with a SuspensionManager key                                
                //
                SuspensionManager.RegisterFrame(RootFrame, "AppFrame");
                //
                // look at previous state
                //
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated || args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
                {
                    //
                    // Restore the saved session state only when appropriate
                    //
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                        //
                        // sync
                        //
                        //ApplicationModel.Current.Synchronize();
                    }
                    catch (SuspensionManagerException)
                    {
                        //
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                        //
                    }
                }
                //
                // Place the frame in the current Window
                //
                Window.Current.Content = RootFrame;
                //
                // set dispatcher
                //
                CoreDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                //
                // get the ConnectionProfile that is currently used to connect to the Internet                
                //
                NetworkInformation.NetworkStatusChanged += async (sender) =>
                {
                    //
                    // get the ConnectionProfile that is currently used to connect to the Internet                
                    //
                    bool connected = NetworkInformation.GetInternetConnectionProfile() != null;
                    //
                    // dispatch on UI thread
                    //
                    await CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //
                        // update
                        //
                        ApplicationModel.Current.IsConnected = connected;
                    });
                };
            }
            if (RootFrame.Content == null)
            {
                //
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                //
                if (!RootFrame.Navigate(typeof(HomePage), null))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            //
            // Ensure the current window is active
            //
            Window.Current.Activate();
            //
            // load state
            //
            ApplicationModel.Current.LoadState();
            //
            // load settings
            //
            ApplicationModel.Current.LoadPersistentSettings();
            //
            // synchronize
            //
            ApplicationModel.Current.Synchronize();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //
            // ensure intialization
            //
            EnsureInitialized(Window.Current.Content as Frame, args);
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            //
            // ensure intialization
            //
            EnsureInitialized(null, args);
            //
            // check
            //
            if (RootFrame != null)
            {
                //
                // If the app is already running and uses top-level frame navigation we can just
                // navigate to the search results
                //
                RootFrame.Navigate(typeof(SearchResultsPage1), args.QueryText);
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private void App_QuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            //
            // check
            //
            if (RootFrame != null)
            {
                //
                // If the app is already running and uses top-level frame navigation we can just
                // navigate to the search results
                //
                RootFrame.Navigate(typeof(SearchResultsPage1), args.QueryText);
            }
        }
    }
}

