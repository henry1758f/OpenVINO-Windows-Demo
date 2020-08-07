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
    public sealed partial class Segmentation_demo : Page
    {
        List<Combobox_models> combobox0_Models = new List<Combobox_models>();
        List<string> filter_model0 = new List<string> { "segmentation" };
        List<string> filter_filter_model0 = new List<string> { };
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
        string[] Input_Source_subname_support = new string[] { ".jpg", ".jpeg", ".png", ".mp4" , ".mov" };

        private async void get_all_and_downloaded_models()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            try
            {
                string inf_file_name = "downloaded_models_list.inf";
                string json_file_name = "models_info.json";

                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(inf_file_name) != null)
                {
                    StorageFile infFile = await ApplicationData.Current.LocalFolder.GetFileAsync(inf_file_name);
                    string inf_string = await FileIO.ReadTextAsync(infFile);
                    List<string> model = inf_string.Split("\n").ToList();
                    foreach (string model_path in model)
                    {
                        if (model_path.Length <= 0)
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
                                    if (_Models.model_name.Contains(filter_filter_word))
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
                MessageDialog messageDialogs = new MessageDialog(e.Message, resourceLoader.GetString("Debug"));
                await messageDialogs.ShowAsync();
            }
        }

        #region CameraPreview
        MediaCapture mediaCapture;
        bool isPreviewing, cam_init = false, previewing = false;
        DisplayRequest displayRequest = new DisplayRequest();
        MediaCapture mediaCaptureMgr = new MediaCapture();


        private async Task StartPreviewAsync()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
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
                var messageDialog = new MessageDialog(resourceLoader.GetString("Camera_Access_Denied_info"), resourceLoader.GetString("Error") + " !");
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
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
            {
                var messageDialog = new MessageDialog(resourceLoader.GetString("Camera_Access_Denied_info"), resourceLoader.GetString("Error") + " !");
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
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
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
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("Selected_File_Empty"), resourceLoader.GetString("Warning") + " !");
                messageDialogs.ShowAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
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
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("You_have_to") + model0_TextBlock.Text, resourceLoader.GetString("Error") + " !");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                Combobox_models model = (Combobox_models)model0_name.SelectedItem;
                Parameter = " -m \"" + model.model_path + "\" ";
                if (model0_target.SelectedItem != null)
                {
                    Parameter += " -d " + model0_target.SelectedItem.ToString() + " ";
                }
            }
            if (Source.Text.Equals(null) || Source.Text.Equals("cam") || Source.Text.Equals(""))
            {
                Parameter += " -i 0";
            }
            else if (Source.Text.Contains(" "))
            {
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("Path_without_space_inform"), resourceLoader.GetString("Error") + " !");
                messageDialogs.ShowAsync();
                //Parameter += " -i \"" + Source.Text.ToString() + "\" ";
                return;
            }
            else
            {
                Parameter += " -i \"" + Source.Text + "\" ";
            }
            // Send Request to ConsoleConnector
            await ((App)Application.Current).SendRequestToConsoleConnector("Segmentation_Demo_Page", Parameter);
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
        public Segmentation_demo()
        {
            this.InitializeComponent();
            get_all_and_downloaded_models();
        }

    }
}
