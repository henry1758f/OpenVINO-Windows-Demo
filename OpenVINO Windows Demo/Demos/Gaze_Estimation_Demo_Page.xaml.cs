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
    public sealed partial class Gaze_Estimation_Demo_Page : Page
    {
        List<Models> Total_models = new List<Models>();
        //List<Models> Downloaded_models = new List<Models>();
        //List<Models> model0_list = new List<Models>();
        List<Combobox_models> combobox0_Models = new List<Combobox_models>();
        List<string> filter_model0 = new List<string> { "gaze-estimation" };
        List<string> filter_filter_model0 = new List<string> { };
        const string model0_arg_parse = "";
        List<Combobox_models> combobox1_Models = new List<Combobox_models>();
        List<string> filter_model1 = new List<string> { "face-detection" };
        List<string> filter_filter_model1 = new List<string> { "face-detection-0105", "face-detection-0106" };
        const string model1_arg_parse = "_fd";
        List<Combobox_models> combobox2_Models = new List<Combobox_models>();
        List<string> filter_model2 = new List<string> { "head-pose-estimation" };
        List<string> filter_filter_model2 = new List<string> { };
        const string model2_arg_parse = "_hp";
        List<Combobox_models> combobox3_Models = new List<Combobox_models>();
        List<string> filter_model3 = new List<string> { "facial-landmarks-35-adas" };
        List<string> filter_filter_model3 = new List<string> { };
        const string model3_arg_parse = "_lm";
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
        const string connector_commandstr = "Gaze_Estimation_Demo_Page";

        private async void get_all_and_downloaded_models()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            try
            {
                //StorageFolder rootFolder = await StorageFolder.GetFolderFromPathAsync(Windows.ApplicationModel.Package.Current.InstalledLocation.Path);
                string inf_file_name = "downloaded_models_list.inf";
                string json_file_name = "models_info.json";
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(json_file_name) != null)
                {
                    StorageFile jsonFile = await ApplicationData.Current.LocalFolder.GetFileAsync(json_file_name);
                    string json_string = await FileIO.ReadTextAsync(jsonFile);

                    List<Models> models = new List<Models>();
                    Total_models = JsonConvert.DeserializeObject<List<Models>>(json_string);
                }
                else
                {
                    MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("Models_Info_Read_Failed") + "\n" + resourceLoader.GetString("Cannot_read") + ApplicationData.Current.LocalFolder.Path + "\\" + json_file_name + " !!", resourceLoader.GetString("Error") + " !");
                    await messageDialogs.ShowAsync();
                }

                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(inf_file_name) != null)
                {
                    StorageFile infFile = await ApplicationData.Current.LocalFolder.GetFileAsync(inf_file_name);
                    string inf_string = await FileIO.ReadTextAsync(infFile);
                    List<string> model = inf_string.Split("\n").ToList();
                    //List<Models> models = new List<Models>();
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
                        foreach (string filter_word in filter_model1)
                        {
                            if (_Models.model_name.Contains(filter_word))
                            {
                                bool filter_filter_trigger = false;
                                foreach (string filter_filter_word in filter_filter_model1)
                                {
                                    if (_Models.model_name.Contains(filter_filter_word))
                                    {
                                        filter_filter_trigger = true;
                                    }
                                }
                                if (!filter_filter_trigger)
                                {
                                    combobox1_Models.Add(_Models);
                                }
                            }
                        }
                        foreach (string filter_word in filter_model2)
                        {
                            if (_Models.model_name.Contains(filter_word))
                            {
                                bool filter_filter_trigger = false;
                                foreach (string filter_filter_word in filter_filter_model2)
                                {
                                    if (_Models.model_name.Contains(filter_filter_word))
                                    {
                                        filter_filter_trigger = true;
                                    }
                                }
                                if (!filter_filter_trigger)
                                {
                                    combobox2_Models.Add(_Models);
                                }
                            }
                        }
                        foreach (string filter_word in filter_model3)
                        {
                            if (_Models.model_name.Contains(filter_word))
                            {
                                bool filter_filter_trigger = false;
                                foreach (string filter_filter_word in filter_filter_model3)
                                {
                                    if (_Models.model_name.Contains(filter_filter_word))
                                    {
                                        filter_filter_trigger = true;
                                    }
                                }
                                if (!filter_filter_trigger)
                                {
                                    combobox3_Models.Add(_Models);
                                }
                            }
                        }
                    }
                }
                model0_name.ItemsSource = combobox0_Models;
                model0_name.DisplayMemberPath = "model_name_show";
                model1_name.ItemsSource = combobox1_Models;
                model1_name.DisplayMemberPath = "model_name_show";
                model2_name.ItemsSource = combobox2_Models;
                model2_name.DisplayMemberPath = "model_name_show";
                model3_name.ItemsSource = combobox3_Models;
                model3_name.DisplayMemberPath = "model_name_show";

            }
            catch (Exception e)
            {
                MessageDialog messageDialogs = new MessageDialog(e.Message, resourceLoader.GetString("Debug"));
                await messageDialogs.ShowAsync();
            }
        }

        public Gaze_Estimation_Demo_Page()
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
                Parameter += " -m" + model0_arg_parse + " \"" + model.model_path + "\" ";
                if (model0_target.SelectedItem != null)
                {
                    Parameter += " -d" + model0_arg_parse + " " + model0_target.SelectedItem.ToString() + " ";
                }
            }
            if (model1_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("You_have_to") + model1_TextBlock.Text, resourceLoader.GetString("Error") + " !");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                Combobox_models model = (Combobox_models)model1_name.SelectedItem;
                Parameter += " -m" + model1_arg_parse + " \"" + model.model_path + "\" ";
                if (model1_target.SelectedItem != null)
                {
                    Parameter += " -d" + model1_arg_parse + " " + model1_target.SelectedItem.ToString() + " ";
                }
            }
            if (model2_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("You_have_to") + model2_TextBlock.Text, resourceLoader.GetString("Error") + " !");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                Combobox_models model = (Combobox_models)model2_name.SelectedItem;
                Parameter += " -m" + model2_arg_parse + " \"" + model.model_path + "\" ";
                if (model2_target.SelectedItem != null)
                {
                    Parameter += " -d" + model2_arg_parse + " " + model2_target.SelectedItem.ToString() + " ";
                }
            }
            if (model3_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("You_have_to") + model3_TextBlock.Text, resourceLoader.GetString("Error") + " !");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                Combobox_models model = (Combobox_models)model3_name.SelectedItem;
                Parameter += " -m" + model3_arg_parse + " \"" + model.model_path + "\" ";
                if (model3_target.SelectedItem != null)
                {
                    Parameter += " -d" + model3_arg_parse + " " + model3_target.SelectedItem.ToString() + " ";
                }
            }
            if (Source.Text.Equals(null) || Source.Text.Equals("cam") || Source.Text.Equals(""))
            {
                Parameter += " -i cam";
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
            await ((App)Application.Current).SendRequestToConsoleConnector(connector_commandstr, Parameter);
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
