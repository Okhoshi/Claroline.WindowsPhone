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

namespace iCampusApp
{
    public partial class DateSwitchPage : PhoneApplicationPage
    {
        public DateSwitchPage()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = (DateTime)DatePicker.Value;
            Connect cx = new Connect();
            if (Connect.cookies.Count == 0)
                cx.Sync("");
            cx.GETPage(@"/claroline/notification_date.php?fday=" + selectedDate.Day + "&fmonth=" + selectedDate.Month + "&fyear=" + selectedDate.Year + "&askBy=index.php");
            //TODO : UPDATE DES STATUS...
            NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}