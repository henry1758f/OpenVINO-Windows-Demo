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
                }
                else
                {

                }
                foreach (string name in model_name)
                {

                }

            }
            catch (Exception e)
            {

            }

        }
        private async void Model_info_Dumper()
        {
            StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
            string json_file_name = "models_info.json";
            if (await rootFolder.TryGetItemAsync(json_file_name) != null)
            {
                StorageFile jsonFile = await rootFolder.GetFileAsync(json_file_name);
                string json_string = await FileIO.ReadTextAsync(jsonFile);

                List<Models> models = new List<Models>();
                Total_Models = JsonConvert.DeserializeObject<List<Models>>(json_string);
                Total_Models_List.ItemsSource = Total_Models;
                

                MessageDialog messageDialogs = new MessageDialog("File " + rootFolder.Path + "\\" + json_file_name + " Exist !! Total " + models.Count.ToString() + " Models!");
                messageDialogs.Title = "DEBUG";
                await messageDialogs.ShowAsync();
                ;


            }
            else
            {
                MessageDialog messageDialogs = new MessageDialog("Cannot Read " + rootFolder.Path + "\\" + json_file_name + " !!");
                messageDialogs.Title = "Failed to Get Models Information";
                await messageDialogs.ShowAsync();
            }
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
            List_All_Models();
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
    }
}
