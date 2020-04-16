using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
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

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private async void Environment_check(string check)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try
            {
                switch (check)
                {
                    case "CPU":
                        Hardware_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        Hardware_info_border.BorderThickness = new Thickness(1);
                        SampleDemo_info.Text = "Checking...";
                        await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "CPU_check");
                        try
                        {
                            Hardware_info.Text = localSettings.Values["CPU"].ToString();
                            if (Hardware_info.Text.Contains("Intel"))
                            {
                                Hardware_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                                Hardware_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                Hardware_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                                Hardware_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                            }
                        }
                        catch (Exception e)
                        {
                            Hardware_info.Text = "ERROR!";
                        }
                        break;
                    case "OpenVINO":
                        OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        OpenVINO_info_border.BorderThickness = new Thickness(1);
                        OpenVINO_info.Text = "Checking...";
                        await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "OpenVINO_check");
                        try
                        {
                            OpenVINO_info.Text = localSettings.Values["OpenVINO"].ToString();
                            if (OpenVINO_info.Text.Contains("openvino_2020.1"))
                            {
                                OpenVINO_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                                OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                OpenVINO_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                                OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                            }
                        }
                        catch (Exception e)
                        {
                            OpenVINO_info.Text = "ERROR!";
                        }
                        break;
                    case "SampleDemo":
                        SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        SampleDemo_info_border.BorderThickness = new Thickness(1);
                        SampleDemo_info.Text = "Checking...";
                        await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "SampleDemo_check");
                        try
                        {

                            if (localSettings.Values["SampleDemo"].ToString().Contains(".exe"))
                            {
                                SampleDemo_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                                SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                                SampleDemo_info.Text = "OK";
                            }
                            else
                            {
                                SampleDemo_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                                SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                                SampleDemo_info.Text = "NOT FOUND";
                            }
                        }
                        catch (Exception e)
                        {
                            SampleDemo_info.Text = "ERROR!" + e.ToString();
                        }
                        break;
                    case "OMZ_Model":
                        OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        OMZ_Model_info_border.BorderThickness = new Thickness(1);
                        OMZ_Model_info.Text = "Checking...";
                        await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "OMZ_Model_check");
                        try
                        {

                            if (localSettings.Values["OMZ_Model"].ToString().Contains(".xml"))
                            {
                                int counter = 0;
                                string temp = localSettings.Values["OMZ_Model"].ToString();
                                while (counter != -1)
                                {
                                    try
                                    {
                                        if (temp.LastIndexOf(".xml") == -1)
                                        {
                                            break;
                                        }
                                        temp = temp.Substring(temp.IndexOf(".xml") + ".xml".Length);
                                        counter++;
                                    }
                                    catch
                                    {
                                        counter = -1;
                                    }

                                }
                                OMZ_Model_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                                OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                                OMZ_Model_info.Text = counter.ToString() + " IR Models have been found.";

                            }
                            else
                            {
                                OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                                OMZ_Model_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                                OMZ_Model_info.Text = "NOT FOUND";
                            }
                        }
                        catch (Exception e)
                        {
                            OMZ_Model_info.Text = "ERROR!" + e.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                MessageDialog messageDialogs = new MessageDialog("ERROR! (" + e.ToString() + ")");
                messageDialogs.ShowAsync();
            }
            

            


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            version_textbox.Text = "Version " + GetAppVersion();
            Environment_check("CPU");
            Environment_check("OpenVINO");
            Environment_check("SampleDemo");
            Environment_check("OMZ_Model");
        }
    }
}
