using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Newtonsoft.Json;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo.Demos
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class Human_Pose_Estimation_Demo_Page : Page
    {
        List<Models> Total_models = new List<Models>();
        List<Models> Downloaded_models = new List<Models>();
        List<Models> model0_list = new List<Models>();
        List<Combobox_models> combobox0_Models = new List<Combobox_models>();
        List<string> filter_model0 = new List<string> { "human-pose-estimation-0001" };
        List<string> filter_filter_model0 = new List<string> { "single-human-pose-estimation-0001" };
        private static string Model_rootPath = UserDataPaths.GetDefault().Documents + @"\Intel\openvino_models\";
        private class Combobox_models
        {
            public string model_path { get; set; }
            public string model_name { get; set; }
            public string model_name_show { get; set; }
            public string model_preprecisions { get; set; }
            public Combobox_models(string path)
            {
                model_path = path;
                string precision = model_path.Substring(0, model_path.LastIndexOf("\\"));
                precision = precision.Substring(precision.LastIndexOf("\\") + 1);
                model_name = model_path.Substring(model_path.LastIndexOf("\\") + 1);
                model_preprecisions = precision;
                model_name_show = model_name + "\t [" + model_preprecisions + "] ";
            }
        }

        string[] Target_device_list = new string[] { "CPU", "GPU", "MYRIAD" };
        string[] Input_Source_subname_support = new string[] { ".jpg", ".jpeg", ".png", ".mp4" };

        private async void get_all_and_downloaded_models()
        {
            try
            {
                StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
                string inf_file_name = "downloaded_models_list.inf";
                string json_file_name = "models_info.json";
                if (await rootFolder.TryGetItemAsync(json_file_name) != null)
                {
                    StorageFile jsonFile = await rootFolder.GetFileAsync(json_file_name);
                    string json_string = await FileIO.ReadTextAsync(jsonFile);

                    List<Models> models = new List<Models>();
                    Total_models = JsonConvert.DeserializeObject<List<Models>>(json_string);
                }
                else
                {
                    MessageDialog messageDialogs = new MessageDialog("Cannot Read " + rootFolder.Path + "\\" + json_file_name + " !!");
                    messageDialogs.Title = "Failed to Get Models Information";
                    await messageDialogs.ShowAsync();
                    
                }

                if (await rootFolder.TryGetItemAsync(inf_file_name) != null)
                {
                    StorageFile infFile = await rootFolder.GetFileAsync(inf_file_name);
                    string inf_string = await FileIO.ReadTextAsync(infFile);
                    List<string> model = inf_string.Split("\n").ToList();
                    //List<Models> models = new List<Models>();
                    foreach (string model_path in model)
                    {
                        if (model_path.Length <=0)
                        {
                            continue;
                        }
                        Combobox_models _Models = new Combobox_models(model_path);
                        foreach (string filter_word in filter_model0)
                        {
                            if (_Models.model_name.Contains(filter_word))
                            {
                                bool filter_filter_trigger = false;
                                foreach (string filter_filter_word in filter_filter_model0)
                                {
                                    if(_Models.model_name.Contains(filter_filter_word))
                                    {
                                        filter_filter_trigger = true;
                                    }
                                }
                                if (!filter_filter_trigger)
                                {
                                    combobox0_Models.Add(_Models);
                                }
                            }
                        }
                    }
                }
                model0_name.ItemsSource = combobox0_Models;
                model0_name.DisplayMemberPath = "model_name_show";
                
            }
            catch (Exception e)
            {

            }
        }
        
        public Human_Pose_Estimation_Demo_Page()
        {
            this.InitializeComponent();
            get_all_and_downloaded_models();
        }

        #region CameraPreview
        MediaCapture mediaCapture;
        bool isPreviewing, cam_init = false, previewing = false;
        DisplayRequest displayRequest = new DisplayRequest();
        MediaCapture mediaCaptureMgr = new MediaCapture();


        private async Task StartPreviewAsync()
        {
            try
            {

                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                var messageDialog = new MessageDialog("The app was denied access to the camera");
                await messageDialog.ShowAsync();
                return;
            }

            try
            {
                PreviewControl.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (System.IO.FileLoadException)
            {
                mediaCapture.CaptureDeviceExclusiveControlStatusChanged += _mediaCapture_CaptureDeviceExclusiveControlStatusChanged;
            }

        }
        private async void _mediaCapture_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        {
            if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
            {
                var messageDialog = new MessageDialog("The camera preview can't be displayed because another app has exclusive access");
                await messageDialog.ShowAsync();
            }
            else if (args.Status == MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable && !isPreviewing)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await StartPreviewAsync();
                });
            }
        }
        private async Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (displayRequest != null)
                    {
                        displayRequest.RequestRelease();
                    }

                    mediaCapture.Dispose();
                    mediaCapture = null;
                });
            }

        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }
        #endregion

        private async void Source_select_button_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            foreach (string type in Input_Source_subname_support)
            {
                picker.FileTypeFilter.Add(type);
            }
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                Source.Text = file.Path;
            }
            else
            {
                MessageDialog messageDialogs = new MessageDialog("No File been selected!");
                messageDialogs.ShowAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string Parameter = "";
            // Close camera and resource 
            if (cam_init && previewing)
            {
                await mediaCaptureMgr.StopPreviewAsync();
                await CleanupCameraAsync();
                previewing = false;

            }
            // Build Parameter String
            if (model0_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to " + model0_TextBlock.Text, "ERROR!");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                Combobox_models model = (Combobox_models)model0_name.SelectedItem;
                Parameter = " -m " + model.model_path + " ";
                if(model0_target.SelectedItem != null)
                {
                    Parameter += " -d " + model0_target.SelectedItem.ToString() + " ";
                }
            }
            if (Source.Text.Equals(null) || Source.Text.Equals("cam") || Source.Text.Equals(""))
            {
                Parameter += " -i cam";
            }
            else if (Source.Text.Contains(" "))
            {
                MessageDialog messageDialogs = new MessageDialog("Please done pick a file path contains space!");
                messageDialogs.ShowAsync();
                //Parameter += " -i \"" + Source.Text.ToString() + "\" ";
                return;
            }
            else
            {
                Parameter += " -i " + Source.Text + " ";
            }
            // Send Request to ConsoleConnector
            await ((App)Application.Current).SendRequestToConsoleConnector("Human_Pose_Estimation_Demo_Page", Parameter);
        }
        private async void Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            // Using Windows.Media.Capture.MediaCapture APIs 
            // to stream from webcam
            if (!cam_init)
            {
                await mediaCaptureMgr.InitializeAsync();
                cam_init = true;
            }

            // Start capture preview.                

            if (!previewing)
            {
                PreviewControl.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();
                previewing = true;
            }
        }
    }
}
