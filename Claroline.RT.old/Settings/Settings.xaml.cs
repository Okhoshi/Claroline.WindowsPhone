using Callisto.Controls;
using ClarolineApp.RT.Common;
using ClarolineApp.RT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClarolineApp.RT.Settings
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Settings : UserControl
    {

        // The guidelines recommend using 100px offset for the content animation.
        const int ContentAnimationOffset = 100;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        HomePage rootPage = HomePage.Current;

        public Settings()
        {
            this.InitializeComponent();
            FlyoutContent.Transitions = new TransitionCollection();
            FlyoutContent.Transitions.Add(new EntranceThemeTransition()
            {
                FromHorizontalOffset = (SettingsPane.Edge == SettingsEdgeLocation.Right) ? ContentAnimationOffset : (ContentAnimationOffset * -1)
            });
            FlyoutContent.DataContext = ApplicationModel.Current;

            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
        }

        /// <summary>
        /// This is the click handler for the back button on the Flyout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        private void onCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand command = new SettingsCommand("CompteId", "Compte", (x) =>
            {
                SettingsFlyout settings = new SettingsFlyout();

                settings.Content = this;
                settings.HeaderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 77, 96));
                settings.Background = new SolidColorBrush(Colors.White);
                settings.HeaderText = "Compte";
                settings.Width = 646;
                settings.IsOpen = true;
            });
            args.Request.ApplicationCommands.Add(command);
            // and so on.. add the other commands you need to show 
        }
    }
}



    