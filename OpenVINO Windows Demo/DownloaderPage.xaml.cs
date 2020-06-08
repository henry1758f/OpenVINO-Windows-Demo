using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Windows.UI.Popups;
using Windows.Storage;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class DownloaderPage : Page
    {
        public DownloaderPage()
        {
            List_All_Models();
            this.InitializeComponent();

        }
        public List<string> model_name = new List<string>();
        public List<Models> Total_Models = new List<Models>();
        public List<Models> Available_Models = new List<Models>();
        //public Models[] Available_Models { get; } = new Models[] { };

        private async void List_All_Models()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            await ((App)Application.Current).SendRequestToConsoleConnector("App_Path", Directory.GetCurrentDirectory() );
            await ((App)Application.Current).SendRequestToConsoleConnector("Downloader", "Get_All_Model_info_json");
            try
            {
                string model_info_result = localSettings.Values["Get_All_Model_info_json"].ToString();
                if (model_info_result.Equals("SUCCESS"))
                {
                    Model_info_Dumper();
                    Load_All_Model_ProgressRing.IsActive = false;
                    Load_All_Model_ProgressRing.Visibility = Visibility.Collapsed;
                    Load_All_Model_TextBlock.Visibility = Visibility.Collapsed;
                    List_Downloaded_Models();
                }
                else
                {
                    Load_All_Model_TextBlock.Text = "ERROR!! Cannot Get Model info.";
                    Load_All_Model_ProgressRing.IsActive = false;
                    Load_All_Model_ProgressRing.Visibility = Visibility.Visible;
                    Load_All_Model_TextBlock.Visibility = Visibility.Visible;
                }
                Download_Progress.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {

            }

        }
        private async void List_Downloaded_Models()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            
            //StorageFolder ModelsFolder = await StorageFolder.GetFolderFromPathAsync(KnownFolders.DocumentsLibrary.Path + @"\Intel\OpenVINO\openvino_models\");
            await ((App)Application.Current).SendRequestToConsoleConnector("Downloader", "Downloaded_Model_Name");
            try
            {
                //StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
                string inf_file_name = "downloaded_models_list.inf";
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(inf_file_name) != null)
                {
                    StorageFile infFile = await ApplicationData.Current.LocalFolder.GetFileAsync(inf_file_name);
                    string inf_string = await FileIO.ReadTextAsync(infFile);
                    List<string> model = inf_string.Split("\n").ToList();
                    List<Models> models = new List<Models>();
                    if (!inf_string.Contains(".xml"))
                    {
                        Load_Downloaded_Model_TextBlock.Text = "No NN Model detected!!";
                        Load_Downloaded_Model_ProgressRing.IsActive = false;
                        Load_Downloaded_Model_ProgressRing.Visibility = Visibility.Visible;
                        Load_Downloaded_Model_TextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        foreach (string name in model)
                        {
                            if (name.Length.Equals(0))
                            {
                                continue;
                            }
                            string precision = name.Substring(0, name.LastIndexOf("\\"));
                            precision = precision.Substring(precision.LastIndexOf("\\") + 1);
                            string pure_name = name.Substring(name.LastIndexOf("\\") + 1);
                            Models model_download = new Models();
                            bool is_inCompare = false;
                            foreach (Models Model_Compare in Total_Models)
                            {
                                if (pure_name.Equals(Model_Compare.name + ".xml"))
                                {
                                    is_inCompare = true;
                                    model_download = Model_Compare;
                                    break;
                                }
                            }
                            if (is_inCompare)
                            {
                                model_download.name = pure_name.Replace(".xml", "");
                                bool is_loaded = false;
                                foreach (Models models_check in models)
                                {

                                    if (model_download.name == models_check.name)
                                    {
                                        is_loaded = true;
                                        model_download.memo_str += " " + precision;
                                        break;
                                    }
                                }
                                if (!is_loaded)
                                {
                                    model_download.memo_str = "Downloaded/Converted precision: " + precision;
                                    models.Add(model_download);
                                }

                            }
                            else
                            {
                                model_download.name = pure_name;
                                model_download.memo_str = precision;
                                models.Add(model_download);
                            }
                            Available_Models = models;

                        }
                    }
                    
                }
                else
                {
                    MessageDialog messageDialogs = new MessageDialog("Cannot Read " + ApplicationData.Current.LocalFolder.Path + "\\" + inf_file_name + " !!");
                    messageDialogs.Title = "Failed to Get Downloaded Models Information";
                    await messageDialogs.ShowAsync();
                }
                if (Available_Models.Count > 0)
                {
                    Downloaded_Model_List.ItemsSource = Available_Models;
                    Load_Downloaded_Model_ProgressRing.IsActive = false;
                    Load_Downloaded_Model_ProgressRing.Visibility = Visibility.Collapsed;
                    Load_Downloaded_Model_TextBlock.Visibility = Visibility.Collapsed;
                }

                /*
                string model_info_result = localSettings.Values["OMZ_Model"].ToString();
                List<string> model = model_info_result.Split("\n").ToList();
                List<Models> models = new List<Models>();
                if (!model_info_result.Contains(".xml"))
                {
                    Load_Downloaded_Model_TextBlock.Text = "No NN Model detected!!";
                    Load_Downloaded_Model_ProgressRing.IsActive = false;
                    Load_Downloaded_Model_ProgressRing.Visibility = Visibility.Visible;
                    Load_Downloaded_Model_TextBlock.Visibility = Visibility.Visible;
                }
                foreach (string name in model)
                {
                    if (name.Length.Equals(0))
                    {
                        continue;
                    }
                    string precision = name.Substring(0, name.IndexOf("\\"));
                    string pure_name = name.Substring(name.LastIndexOf("\\") + 1);
                    Models model_download = new Models();
                    bool is_inCompare = false;
                    foreach (Models Model_Compare in Total_Models)
                    {
                        if (pure_name.Contains(Model_Compare.name))
                        {
                            is_inCompare = true;
                            model_download = Model_Compare;
                            break;
                        }
                    }
                    if (is_inCompare)
                    {
                        model_download.name = pure_name.Replace(".xml","");
                        bool is_loaded = false;
                        foreach (Models models_check in models)
                        {
                            
                            if (model_download.name == models_check.name)
                            {
                                is_loaded = true;
                                model_download.memo_str += " " + precision;
                                break;
                            }
                        }
                        if (!is_loaded )
                        {
                            model_download.memo_str = "Downloaded/Converted precision: " + precision;
                            models.Add(model_download);
                        }

                    }
                    else
                    {
                        model_download.name = pure_name;
                        model_download.memo_str = precision;
                        models.Add(model_download);
                    }
                    Available_Models = models;
                   
                }
                if (Available_Models.Count >0)
                {
                    Downloaded_Model_List.ItemsSource = Available_Models;
                    Load_Downloaded_Model_ProgressRing.IsActive = false;
                    Load_Downloaded_Model_ProgressRing.Visibility = Visibility.Collapsed;
                    Load_Downloaded_Model_TextBlock.Visibility = Visibility.Collapsed;
                }*/

            }
            catch (Exception e)
            {
                Load_Downloaded_Model_TextBlock.Text = "ERROR!! Cannot Get Model info.";
                Load_Downloaded_Model_ProgressRing.IsActive = false;
                Load_Downloaded_Model_ProgressRing.Visibility = Visibility.Visible;
                Load_Downloaded_Model_TextBlock.Visibility = Visibility.Visible;
            }
        }
        private async void Model_info_Dumper()
        {
            Download_Status_Textblock.Text = "Getting Model Informations...";
            //StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
            string json_file_name = "models_info.json";
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(json_file_name) != null)
            {
                StorageFile jsonFile = await ApplicationData.Current.LocalFolder.GetFileAsync(json_file_name);
                string json_string = await FileIO.ReadTextAsync(jsonFile);

                List<Models> models = new List<Models>();
                Total_Models = JsonConvert.DeserializeObject<List<Models>>(json_string);
                foreach (Models model in Total_Models)
                {
                    model.memo_str = "Support precisions: ";
                    foreach (string precision in model.precisions)
                    {
                        model.memo_str += precision + " ";
                    }
                }
                Total_Models_List.ItemsSource = Total_Models;
                

                //MessageDialog messageDialogs = new MessageDialog("File " + rootFolder.Path + "\\" + json_file_name + " Exist !! Total " + models.Count.ToString() + " Models!");
                //messageDialogs.Title = "DEBUG";
                //await messageDialogs.ShowAsync();
                ;


            }
            else
            {
                MessageDialog messageDialogs = new MessageDialog("Cannot Read " + ApplicationData.Current.LocalFolder.Path + "\\" + json_file_name + " !!");
                messageDialogs.Title = "Failed to Get Models Information";
                await messageDialogs.ShowAsync();
            }
            Download_Status_Textblock.Text = "";
            /*
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            await ((App)Application.Current).SendRequestToConsoleConnector("Get_All_Model_info_json", name);
            try
            {
                List<Models> models = new List<Models>();
                string model_info_str = localSettings.Values["Model_Info"].ToString();
                models = JsonConvert.DeserializeObject<List<Models>>(model_info_str);
                Total_Models.Add(models[0]);
            }
            catch (Exception e)
            {

            }*/
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            //List_All_Models();
        }

        private async void Download_All_Button_Click(object sender, RoutedEventArgs e)
        {
            Download_Status_Textblock.Text = "Downloading ALL Models";
            Total_Models_List.SelectAll();
            int counter = Total_Models_List.SelectedItems.Count;
            MessageDialog messageDialogs = new MessageDialog("You are going to download ALL " + counter + " models!! \n After Downloading and converting, it will take you about 36 GB on your devices (23 GB to downloaded))!!");
            messageDialogs.Title = "Model Download and Convert";
            messageDialogs.Commands.Add(new UICommand("Accept", new UICommandInvokedHandler(this.MsgBox_CommandInvokedHandler)));
            messageDialogs.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.MsgBox_CommandInvokedHandler)));
            // Set the command that will be invoked by default
            messageDialogs.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialogs.CancelCommandIndex = 1;
            // Show the message dialog
            await messageDialogs.ShowAsync();
        }

        private async void Download_Selected_Button_Click(object sender, RoutedEventArgs e)
        {
            int counter = Total_Models_List.SelectedItems.Count;
            MessageDialog messageDialogs = new MessageDialog("You are going to download " + counter + " models!!");
            messageDialogs.Title = "Model Download and Convert";
            messageDialogs.Commands.Add(new UICommand("Accept", new UICommandInvokedHandler(this.MsgBox_CommandInvokedHandler)));
            messageDialogs.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.MsgBox_CommandInvokedHandler)));
            // Set the command that will be invoked by default
            messageDialogs.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialogs.CancelCommandIndex = 1;
            // Show the message dialog
            await messageDialogs.ShowAsync();
        }
        private void MsgBox_CommandInvokedHandler(IUICommand command)
        {
            switch (command.Label)
            {
                case "Accept":
                    Downloader();
                    break;
                case "Cancel":

                    break;
                default:
                    break;
            }
        }
        private async void Downloader()
        {
            Download_Progress.Visibility = Visibility.Visible;
            int counter = Total_Models_List.SelectedItems.Count;
            if (counter > 0)
            {
                Download_Info_Textblock.Visibility = Visibility.Visible;

                List<string> model_name = new List<string>();
                //MessageDialog messageDialogs = new MessageDialog("You are going to download " + counter + " models!!");
                //messageDialogs.Title = "Model Download and Convert";
                //messageDialogs.Commands.Add(new UICommand("Accept"))
                //await messageDialogs.ShowAsync();
                //await ((App)Application.Current).SendRequestToConsoleConnector("Command", "prerequest_DOWNLOADER");
                foreach (Models item in Total_Models_List.SelectedItems)
                {
                    model_name.Add(item.name);
                }
                int i = 0;
                foreach (string name in model_name)
                {
                    Download_Status_Textblock.Text = "Downloading Models...";
                    Download_Info_Textblock.Text = "Downloading " + (++i).ToString() + " of " + counter.ToString() + " Models...";
                    Download_Progress.Value = (i-1)*100 / counter;
                    if (Download_Progress.Value > 0)
                    {
                        Download_Progress.IsIndeterminate = false;
                    }
                    Download_Progress.UpdateLayout();
                    await ((App)Application.Current).SendRequestToConsoleConnector("Downloader", "DOWNLOAD " + name);
                    List_Downloaded_Models();
                }
                Download_Progress.Value = 100;
                Download_Status_Textblock.Text = "Download Finish!";
                Download_Info_Textblock.Text = counter + " models downloaded.";
            }
        }
    }
    public class Models
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("framework")]
        public string framework { get; set; }
        [JsonProperty("license_url")]
        public string license_url { get; set; }
        [JsonProperty("precisions")]
        public string[] precisions { get; set; }
        [JsonProperty("subdirectory")]
        public string subdirectory { get; set; }
        [JsonProperty("task_type")]
        public string task_type { get; set; }
        public string memo_str { get; set; }
    }
}
