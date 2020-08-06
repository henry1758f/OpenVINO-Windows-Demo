using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            model0_name.PlaceholderText = localSettings.Values["default_model_0"].ToString();
            model0_name.Text = localSettings.Values["model_0"].ToString();
            model0_target.PlaceholderText = localSettings.Values["default_model_0_target"].ToString();
            model0_target.Text = localSettings.Values["model_0_target"].ToString();

            model1_name.PlaceholderText = localSettings.Values["default_model_1"].ToString();
            model1_name.Text = localSettings.Values["model_1"].ToString();
            model1_target.PlaceholderText = localSettings.Values["default_model_1_target"].ToString();
            model1_target.Text = localSettings.Values["model_1_target"].ToString();

            model2_name.PlaceholderText = localSettings.Values["default_model_2"].ToString();
            model2_name.Text = localSettings.Values["model_2"].ToString();
            model2_target.PlaceholderText = localSettings.Values["default_model_2_target"].ToString();
            model2_target.Text = localSettings.Values["model_2_target"].ToString();

            fg_Path.PlaceholderText = localSettings.Values["default_fg"].ToString();
            fg_Path.Text = localSettings.Values["fg"].ToString();
        }

        private async void connector_show_toggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (connector_show_toggle.IsOn)
            {
                await ((App)Application.Current).SendRequestToConsoleConnector("Command", "connector_SHOW");
            }
            else
            {
                await ((App)Application.Current).SendRequestToConsoleConnector("Command", "connector_HIDE");
            }
        }

        private async void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            if (azs_iothub.Text.Length == 0 || azs_storage.Text.Length == 0)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to set connection String!!", "Missing Config");
                await messageDialogs.ShowAsync();
            }
            else
            {
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["iot_device_id"] = "";
                localSettings.Values["azure_iot_hub_connection_string"] = azs_iothub.Text;
                localSettings.Values["azure_storage_connection_string"] = azs_storage.Text;
                localSettings.Values["azure_delay_time"] = Aztime_delay.Text;
            }
        }
    }
}
