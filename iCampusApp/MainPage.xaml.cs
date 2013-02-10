﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.Linq;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using ClarolineApp.Languages;
using System.Windows.Data;

namespace ClarolineApp
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        PropertyChangedEventHandler MainPage_Handler;
        PropertyChangedEventHandler Failure_Handler;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            //MainPage_Handler = new PropertyChangedEventHandler(MainPage_PropertyChanged);
            //Failure_Handler = new PropertyChangedEventHandler(FailureOccured);
        }
        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------

/*
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Panorama.Title = set.PlatformSetting;

            if (set.UsernameSetting == "")
            {
                if (MessageBox.Show(AppLanguage.ErrorMessage_NoLogin_Message, AppLanguage.ErrorMessage_NoLogin_Caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
                }
            }

        }

        private void CoursList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoursList.SelectedIndex == -1)
            {
                return;
            }

            App.selecteditem = CoursList.SelectedItem;

            if ((CoursList.SelectedItem as Cours).loadedToday())
            {
                if((CoursList.SelectedItem as Cours).notified)
                    (CoursList.SelectedItem as Cours).checkNotified();
                NavigationService.Navigate(new Uri("/CoursPage.xaml", UriKind.Relative));
            }
            else
            {
                ((Cours)CoursList.SelectedItem).PropertyChanged += MainPage_Handler;
                App.Client.PropertyChanged += Failure_Handler;

                PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.SelectedIndex) as ListBoxItem);
                progress.Visibility = System.Windows.Visibility.Visible;

                App.Client.makeOperation(AllowedOperations.updateCompleteCourse, CoursList.SelectedItem as Cours);
                App.selecteditem = CoursList.SelectedItem;
            }
            CoursList.SelectedIndex = -1;
        }

        void FailureOccured(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Failure" && App.selecteditem != null)
            {
                Cours _cours = App.selecteditem as Cours;
                App.selecteditem = null;
                _cours.PropertyChanged -= MainPage_Handler;
                App.Client.PropertyChanged -= Failure_Handler;

                PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.Items.IndexOf(_cours)) as ListBoxItem);
                progress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void MainPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "isLoaded":
                    ((Cours)sender).PropertyChanged -= MainPage_Handler;

                    PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.Items.IndexOf(sender)) as ListBoxItem);
                    progress.Visibility = System.Windows.Visibility.Collapsed;
                    App.selecteditem = sender;

                    if ((sender as Cours).notified)
                    {
                        ((Cours)sender).notified = false;
                        App.ViewModel.AddCours((Cours)sender);
                    }
                    NavigationService.Navigate(new Uri("/CoursPage.xaml", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }

        private void SettingsPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getCourseList);
        }

        private void Sync_Btn_Click(object sender, EventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getUpdates);
        }

        private void DEV_clrDB_Click(object sender, EventArgs e)
        {
            App.ViewModel.ResetDatabase();
        }

        private void IEButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask homepage = new WebBrowserTask()
            {
                Uri = new Uri(set.DomainSetting + set.AuthPageSetting + "?login=" + set.UsernameSetting + "&password=" + set.PasswordSetting, UriKind.Absolute)
            };
            homepage.Show();
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask mailToDev = new EmailComposeTask()
            {
                To = String.Format("\"{0} Development Team\" <support.develop@live.be>", App.ApplicationName),
                Subject = String.Format("[{0}] {1}", App.ApplicationName, AppLanguage.MainPage_About_DevMailButton),
				Body = String.Format("[V:{0} Campus:{1}]\n\n", Helper.GetVersionNumber(true), set.PlatformSetting)
            };
            mailToDev.Show();
        }

        private void sendMail_Click(object sender, RoutedEventArgs e)
        {
            Cours _item = (sender as MenuItem).Tag as Cours;
            if (_item != null)
            {
                EmailComposeTask mailToManagers = new EmailComposeTask()
                {
                    To = String.Format("\"{0}\" <{1}>", _item.titular, _item.officialEmail),
                    Subject = String.Format("[{0}][{1}]", set.PlatformSetting, _item.title)
                };
                mailToManagers.Show();
            }
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Panorama.SelectedItem.Equals(Cours_PI))
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            Notification note = e.Item as Notification;
            e.Accepted = !(note == null || note.date.CompareTo(DateTime.Now.AddDays(-7)) <= 0);
        }

        private void NotifList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotifList.SelectedIndex == -1 || set == null)
            {
                return;
            }

            Notification notif = NotifList.SelectedItem as Notification;
            notif.checkNotified();
            NotifList.SelectedIndex = -1;
            switch (notif.ressourceType)
            {
                case ValidTypes.Annonce:
                    Annonce ressource = App.ViewModel.AllAnnonces.Single(ann => ann.ressourceId == notif.ressourceId);
                    App.selecteditem = ressource;
                    NavigationService.Navigate(new Uri("/AnnonceDetail.xaml", UriKind.Relative));
                    break;
                case ValidTypes.Documents:
                    Documents ressource_doc = App.ViewModel.AllFiles.Single(doc => doc.Id == notif.ressourceId);
                    if (ressource_doc.notified)
                    {
                        ressource_doc.notified = false;
                        App.ViewModel.AddDocument(ressource_doc);
                        ressource_doc.Cours.checkNotified();
                    }
                    break;
            }
        }
  */

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}