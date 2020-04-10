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

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo.Demos
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class Gaze_Estimation_Demo_Page : Page
    {
        const string user_source_select_inform = "select a picture/video";
        string[] Target_device_list = new string[] { "CPU", "GPU", "MYRIAD" };
        string[] Input_Source = new string[] { "cam", user_source_select_inform };
        string[] Input_Source_subname_support = new string[] { ".jpg", ".jpeg", ".png", ".mp4" };

        private static string Model_rootPath = @"D:\Intel\openvino_models\";

        private class model_dataList
        {
            public string Model_Name { get; set; }
            public string Model_Path { get; set; }
            public string[] Model_precision_Support { get; set; }
            public string Model_framework { get; set; }
            public string Model_detail { get; set; }
        }
        private static List<model_dataList> model0_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "gaze-estimation-adas-0002",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };
        private static List<model_dataList> model1_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "face-detection-retail-0004",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            },
            new model_dataList{
                Model_Name = "face-detection-adas-0001",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };
        private static List<model_dataList> model2_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "head-pose-estimation-adas-0001",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };
        private static List<model_dataList> model3_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "facial-landmarks-35-adas-0002",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };
        public Gaze_Estimation_Demo_Page()
        {
            this.InitializeComponent();
            foreach (model_dataList model_DataList in model0_list)
            {
                foreach (string precision in model_DataList.Model_precision_Support)
                {
                    model0_name.Items.Add(model_DataList.Model_Name + " [" + precision + "]");
                }
            }
            foreach (model_dataList model_DataList in model1_list)
            {
                foreach (string precision in model_DataList.Model_precision_Support)
                {
                    model1_name.Items.Add(model_DataList.Model_Name + " [" + precision + "]");
                }
            }
            foreach (model_dataList model_DataList in model2_list)
            {
                foreach (string precision in model_DataList.Model_precision_Support)
                {
                    model2_name.Items.Add(model_DataList.Model_Name + " [" + precision + "]");
                }
            }
            foreach (model_dataList model_DataList in model3_list)
            {
                foreach (string precision in model_DataList.Model_precision_Support)
                {
                    model3_name.Items.Add(model_DataList.Model_Name + " [" + precision + "]");
                }
            }
            foreach (string str in Input_Source)
            {
                Source.Items.Add(str);
            }
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
        private async void Source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Source.SelectedItem.ToString().Equals(user_source_select_inform))
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                foreach (string type in Input_Source_subname_support)
                {
                    picker.FileTypeFilter.Add(type);
                }
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    Source.Items.Add(file.Path);
                    Source.SelectedItem = file.Path;
                }
                else
                {
                    MessageDialog messageDialogs = new MessageDialog("No File been selected!");
                    messageDialogs.ShowAsync();
                }
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
                foreach (model_dataList model_DataList in model0_list)
                {
                    foreach (string precision in model_DataList.Model_precision_Support)
                    {
                        if (model0_name.SelectedItem.ToString().Contains(precision) && model0_name.SelectedItem.ToString().Contains(model_DataList.Model_Name))
                        {

                            if (model0_target.SelectedItem != null)
                                Parameter = " -m " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d " + model0_target.SelectedItem.ToString() + " ";
                            else
                                Parameter = " -m " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }
            if (model1_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to " + model1_TextBlock.Text, "ERROR!");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                foreach (model_dataList model_DataList in model1_list)
                {
                    foreach (string precision in model_DataList.Model_precision_Support)
                    {
                        if (model1_name.SelectedItem.ToString().Contains(precision) && model1_name.SelectedItem.ToString().Contains(model_DataList.Model_Name))
                        {

                            if (model1_target.SelectedItem != null)
                                Parameter += " -m_fd " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_fd " + model1_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_fd " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }
            if (model2_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to " + model2_TextBlock.Text, "ERROR!");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                foreach (model_dataList model_DataList in model2_list)
                {
                    foreach (string precision in model_DataList.Model_precision_Support)
                    {
                        if (model2_name.SelectedItem.ToString().Contains(precision) && model2_name.SelectedItem.ToString().Contains(model_DataList.Model_Name))
                        {

                            if (model2_target.SelectedItem != null)
                                Parameter += " -m_hp " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_hp " + model2_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_hp " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }
            if (model3_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to " + model3_TextBlock.Text, "ERROR!");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                foreach (model_dataList model_DataList in model3_list)
                {
                    foreach (string precision in model_DataList.Model_precision_Support)
                    {
                        if (model3_name.SelectedItem.ToString().Contains(precision) && model3_name.SelectedItem.ToString().Contains(model_DataList.Model_Name))
                        {

                            if (model3_target.SelectedItem != null)
                                Parameter += " -m_lm " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_lm " + model3_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_lm " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }

            /*
            if (Source.SelectedItem == null || Source.SelectedItem == "cam")
            {
                Parameter += " -i cam";
            }
            else if (Source.SelectedItem.ToString().Contains(" "))
            {
                Parameter += " -i \"" + Source.SelectedItem.ToString() + "\" ";
            }
            else
            {
                Parameter += " -i " + Source.SelectedItem.ToString() + " ";
            }*/
            // Send Request to ConsoleConnector
            await ((App)Application.Current).SendRequestToConsoleConnector("Gaze_Estimation_Demo_Page", Parameter);
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



            //await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }
    }
}
