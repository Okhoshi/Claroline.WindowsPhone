using System;
using System.ComponentModel;
namespace ClarolineApp.Settings
{
    public interface ISettings : INotifyPropertyChanged
    {
        bool AdvancedSwitchSetting { get; set; }
        string AuthPageSetting { get; set; }
        bool CellularDataEnabledSetting { get; set; }
        string DomainSetting { get; set; }
        string InstituteSetting { get; set; }
        bool IsValidHostSetting { get; set; }
        DateTime LastListRequestSetting { get; set; }
        int ListBoxSetting { get; set; }
        string PasswordSetting { get; set; }
        string PlatformSetting { get; set; }
        bool RadioButton3Setting { get; set; }
        string UserNameSetting { get; set; }
        User UserSetting { get; set; }
        bool UseSSLSetting { get; set; }
        bool TryHTTPSetting { get; set; }
        string WebServiceSetting { get; set; }

        void Reset();

    }
}
