using System;
using System.Collections.Generic;
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
using Microsoft.Phone.Shell;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        ClaroClientTest client;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            SystemTray.SetProgressIndicator(this, App.currentProgressInd);
            client = new ClaroClientTest();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            client.testGetUserId();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            client.testgetCourseList();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            client.testGetDocList();
        }

        private void ListPick_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}