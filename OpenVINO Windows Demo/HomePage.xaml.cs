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

        private async System.Threading.Tasks.Task Environment_check(string check)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool pass = false;
            switch (check)
            {
                case "connector":
                    int connector_counter = 0;
                    
                    while (connector_counter != -1)
                    {
                        Env_check_info.Text = "Checking Console Connector...";
                        connector_counter++;
                        try
                        {
                            await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "ECHO");
                            pass = true;
                            break;
                        }
                        catch (Exception e)
                        {
                            if (connector_counter == 3)
                            {
                                Env_check_info.Text = "Try to Reopen Console Connector...";
                                ((App)Application.Current).APPLaunch();
                                System.Threading.Tasks.Task.Delay(10).Wait();
                                //await ((App)Application.Current).SendRequestToConsoleConnector("Command", "Reopen");
                            }
                            if (connector_counter > 10)
                            {
                                pass = false;
                                break;
                            }
                            continue;
                        }
                    }
                    if (!pass)
                    {
                        Env_check_info.Text = "ERROR! Console Connector is not ready!!";
                    }
                    else
                    {
                        Env_check_info.Text = "";
                    }
                    break;
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
                            Fix_Hardware.Visibility = Visibility.Visible;
                        }
                        pass = true;
                    }
                    catch (Exception e)
                    {
                        Hardware_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                        Hardware_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                        Fix_Hardware.Visibility = Visibility.Visible;
                        pass = false;
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
                        if (OpenVINO_info.Text.Contains("openvino_2020.2"))
                        {
                            OpenVINO_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                            OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                            Fix_OpenVINO.Visibility = Visibility.Collapsed;
                            // DEBUG
                            //Fix_OpenVINO.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            OpenVINO_info.Text += "\nPlease install OpenVINO v2020.2";
                            OpenVINO_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                            OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                            Fix_OpenVINO.Visibility = Visibility.Visible;
                            Fix_SampleDemo.IsEnabled = false;
                            Fix_OMZ_Model.IsEnabled = false;
                        }
                        pass = true;
                    }
                    catch (Exception e)
                    {
                        OpenVINO_info.Text += "No OpenVINO detected!\nPlease install OpenVINO v2020.2";
                        OpenVINO_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                        OpenVINO_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                        Fix_OpenVINO.Visibility = Visibility.Visible;
                        Fix_SampleDemo.IsEnabled = false;
                        Fix_OMZ_Model.IsEnabled = false;
                        pass = false;
                    }
                    break;
                case "SampleDemo":
                    SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    SampleDemo_info_border.BorderThickness = new Thickness(1);
                    SampleDemo_info.Text = "Checking...";
                    if (OpenVINO_info.Text.Contains("openvino_2020.2"))
                    {
                        await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "SampleDemo_check");
                        try
                        {

                            if (localSettings.Values["SampleDemo"].ToString().Contains(".exe"))
                            {
                                SampleDemo_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                                SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                                SampleDemo_info.Text = "OK";
                                Fix_SampleDemo.Visibility = Visibility.Collapsed;
                                // DEBUG
                                //Fix_SampleDemo.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                SampleDemo_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                                SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                                SampleDemo_info.Text = "NOT FOUND";
                                Fix_SampleDemo.Visibility = Visibility.Visible;
                                Fix_SampleDemo.IsEnabled = true;

                            }
                            pass = true;
                        }
                        catch (Exception e)
                        {
                            SampleDemo_info.Text = "ERROR!" + e.ToString();
                            pass = false;
                        }
                    }
                    else
                    {
                        SampleDemo_info.Text = "Please Install the OpenVINO Toolkits.";
                        SampleDemo_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                        SampleDemo_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                        Fix_SampleDemo.Visibility = Visibility.Visible;
                        Fix_SampleDemo.IsEnabled = false;
                    }
                    
                    break;
                case "OMZ_Model":
                    OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                    OMZ_Model_info_border.BorderThickness = new Thickness(1);
                    OMZ_Model_info.Text = "Checking...";
                    string path = Directory.GetCurrentDirectory();
                    string appdata_path = ApplicationData.Current.LocalFolder.Path;
                    await ((App)Application.Current).SendRequestToConsoleConnector("App_Path", path);
                    await ((App)Application.Current).SendRequestToConsoleConnector("AppData_Path", appdata_path);
                    await ((App)Application.Current).SendRequestToConsoleConnector("Home_Page", "OMZ_Model_check");
                    try
                    {
                        StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
                        StorageFolder appdataFolder = ApplicationData.Current.LocalFolder;

                        string downloaded_models_list_inf_file_name = "downloaded_models_list.inf";
                        if (await appdataFolder.TryGetItemAsync(downloaded_models_list_inf_file_name) != null)
                        {
                            StorageFile infFile = await appdataFolder.GetFileAsync(downloaded_models_list_inf_file_name);
                            string inf_string = await FileIO.ReadTextAsync(infFile);
                            OMZ_Model_info.Text = "Find " + localSettings.Values["OMZ_Model"].ToString() + " IR models.";
                            OMZ_Model_Checking_sign.Fill = new SolidColorBrush(Colors.Green);
                            OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Green);
                            Fix_OMZ_Model.Visibility = Visibility.Collapsed;
                            //MessageDialog messageDialogs = new MessageDialog("File " + appdataFolder.Path + "\\" + json_file_name + " Exist !! Total " + models.Count.ToString() + " Models!");
                            //messageDialogs.Title = "DEBUG";
                            //await messageDialogs.ShowAsync();
                            ;
                        }
                        else
                        {
                            OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Red);
                            OMZ_Model_Checking_sign.Fill = new SolidColorBrush(Colors.Red);
                            OMZ_Model_info.Text = "ERROR!";
                            Fix_OMZ_Model.Visibility = Visibility.Visible;
                            MessageDialog messageDialogs = new MessageDialog("Cannot Read " + appdataFolder.Path + "\\" + downloaded_models_list_inf_file_name + " !!");
                            messageDialogs.Title = "Failed to Get downloaded Models Information";
                            await messageDialogs.ShowAsync();
                        }
                        if (localSettings.Values["OMZ_Model"].ToString().Equals("0"))
                        {
                            OMZ_Model_info_border.BorderBrush = new SolidColorBrush(Colors.Yellow);
                            OMZ_Model_Checking_sign.Fill = new SolidColorBrush(Colors.Yellow);
                            Fix_OMZ_Model.Visibility = Visibility.Visible;
                            Fix_OMZ_Model.IsEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        OMZ_Model_info.Text = "ERROR!" + e.ToString();
                        //pass = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            const string connector_commandstr = "Face_Recognition_Demo_Azure_Iot_Page";
            string Parameter = "";
            string model_0 = "";
            string model_1 = "";
            string model_2 = "";
            string az_iothub_str = "";
            string az_storage_str = "";
            string fg_path = "";
            
            Refresh_Button.IsEnabled = false;
            version_textbox.Text = "Version " + GetAppVersion();
            //Environment_check_process(sender,e);
            int counter = 0;
            do
            {
                await Environment_check("connector");
                counter++;
            } while (Env_check_info.Text.Contains("ERROR") && counter <= 0);
            try
            {
                await Environment_check("CPU");
                await Environment_check("OpenVINO");
                //await Environment_check("SampleDemo");
                //await Environment_check("OMZ_Model");
                string path = Directory.GetCurrentDirectory();
                await ((App)Application.Current).SendRequestToConsoleConnector("App_Path", path);
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                model_0 = localSettings.Values["model_0"].ToString();
                model_1 = localSettings.Values["model_1"].ToString();
                model_2 = localSettings.Values["model_2"].ToString();
                fg_path = localSettings.Values["fg"].ToString();
                Parameter = " -m_fd \"" + model_0 + "\" -d_fd " + localSettings.Values["model_0_target"].ToString();
                Parameter += " -m_reid \"" + model_1 + "\" -d_reid " + localSettings.Values["model_1_target"].ToString();
                Parameter += " -m_lm \"" + model_2 + "\" -d_lm " + localSettings.Values["model_2_target"].ToString();
                Parameter += " -fg " + fg_path;
                if (localSettings.Values["azure_iot_hub_connection_string"] == null || localSettings.Values["azure_storage_connection_string"] == null || localSettings.Values["azure_iot_hub_connection_string"].ToString().Length == 0 || localSettings.Values["azure_storage_connection_string"].ToString().Length == 0)
                {
                    MessageDialog messageDialogs = new MessageDialog("You have to set connection String!!", "Missing Config");
                    await messageDialogs.ShowAsync();
                }
                else
                {
                    Parameter += " -azstr_iothub " + localSettings.Values["azure_iot_hub_connection_string"].ToString();
                    Parameter += " -azstr_storage " + localSettings.Values["azure_storage_connection_string"].ToString();
                    //Parameter += " -cw 1920 -ch 1080";
                    await ((App)Application.Current).SendRequestToConsoleConnector(connector_commandstr, Parameter);
                }
                

            }
            catch (Exception ex)
            {
                MessageDialog messageDialogs = new MessageDialog("Console Connector is not ready!!" + ex.ToString());
                messageDialogs.Title = "Console Connetor Problem";
                await messageDialogs.ShowAsync();
                Page_Loaded(sender, e);
            }
            Refresh_Button.IsEnabled = true;
        }

        private async void Fix_Hardware_Click(object sender, RoutedEventArgs e)
        {
            string UriToSYNNEX = @"http://synnex-iotsolutions.com/";
            var uri = new Uri(UriToSYNNEX);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (success)
            {

            }
            else
            {
                MessageDialog messageDialogs = new MessageDialog("Cannot launch the website !!");
                messageDialogs.Title = "Hardware Purcase page";
                await messageDialogs.ShowAsync();
            }
        }

        private async void Fix_OpenVINO_Click(object sender, RoutedEventArgs e)
        {
            string UriToOpenVINO = @"https://software.seek.intel.com/openvino-toolkit?os=windows";
            var uri = new Uri(UriToOpenVINO);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (success)
            {

            }
            else
            {
                MessageDialog messageDialogs = new MessageDialog("Cannot launch the website !!");
                messageDialogs.Title = "OpenVINO Download page";
                await messageDialogs.ShowAsync();
            }
        }

        private async void Fix_SampleDemo_Click(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            await ((App)Application.Current).SendRequestToConsoleConnector("App_Path", path);
            await ((App)Application.Current).SendRequestToConsoleConnector("Command", "Sample_Build");
        }

        private async void Fix_OMZ_Model_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog messageDialogs = new MessageDialog("You donot have any model!!");
            messageDialogs.Title = "OpenVINO Download page";
            await messageDialogs.ShowAsync();
            //MainPage.NavView_Navigate("downloader");
        }

        private void Button_refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh_Button.IsEnabled = false;
            Page_Loaded(sender, e);
            
        }
    }
}
