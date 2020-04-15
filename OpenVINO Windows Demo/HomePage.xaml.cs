using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
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
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }
        int Env_check_counter = 0;
        private async void Environment_check(string check)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            switch (check)
            {
                case "CPU":
                    await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "CPU_check");
                    try
                    {
                        Hardware_info.Text = localSettings.Values["CPU"].ToString();
                        if(Hardware_info.Text.Contains("Intel"))
                        {
                            Hardware_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            Hardware_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                        }
                    }
                    catch (Exception e)
                    {
                        Hardware_info.Text = "ERROR!";
                    }
                    break;
                case "OpenVINO":
                    await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "OpenVINO_check");
                    try
                    {
                        OpenVINO_info.Text = localSettings.Values["OpenVINO"].ToString();
                    }
                    catch (Exception e)
                    {
                        OpenVINO_info.Text = "ERROR!";
                    }
                    break;
                default:
                    break;
            }

            


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            Environment_check("CPU");
            Environment_check("OpenVINO");
        }
    }
}
