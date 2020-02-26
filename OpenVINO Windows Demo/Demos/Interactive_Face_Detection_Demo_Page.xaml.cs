#define DEBUG
//#undef DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.UI.Core;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo.Demos
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class Interactive_Face_Detection_Demo_Page : Page
    {
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
                Model_Name = "face-detection-adas-0001",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            },
            new model_dataList{
                Model_Name = "face-detection-adas-binary-0001",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32-INT1" },
                Model_framework = "dldt"
            }
        };
        private static List<model_dataList> model1_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "age-gender-recognition-retail-0013",
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
                Model_Name = "emotions-recognition-retail-0003",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };
        private static List<model_dataList> model4_list = new List<model_dataList>()
        {
            new model_dataList{
                Model_Name = "facial-landmarks-35-adas-0002",
                Model_Path = @"models\SYNNEX_demo\intel\",
                Model_precision_Support = new string[]{"FP32","FP16", "FP32-INT8" },
                Model_framework = "dldt"
            }
        };

        /*
        private readonly List<(string Model_Name,string Model_Path)> _model0_list = new List<(string Model_Name, string Model_Path)>
        {
            ("face-detection-adas-0001 [FP32]", @"models\SYNNEX_demo\intel\face-detection-adas-0001\FP32\face-detection-adas-0001.xml"),
            ("face-detection-adas-0001 [FP16]", @"models\SYNNEX_demo\intel\face-detection-adas-0001\FP16\face-detection-adas-0001.xml"),
            ("face-detection-adas-0001 [FP32-INT8]", @"models\SYNNEX_demo\intel\face-detection-adas-0001\FP32-INT8\face-detection-adas-0001.xml"),
            ("face-detection-adas-binary-0001 [FP32-INT1]",@"models\SYNNEX_demo\intel\face-detection-adas-binary-0001\FP32-INT1\face-detection-adas-binary-0001.xml")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model1_list = new List<(string Model_Name, string Model_Path)>
        {
            ("age-gender-recognition-retail-0013 [FP32]", @"models\SYNNEX_demo\intel\age-gender-recognition-retail-0013\FP32\age-gender-recognition-retail-0013.xml"),
            ("age-gender-recognition-retail-0013 [FP16]", @"models\SYNNEX_demo\intel\age-gender-recognition-retail-0013\FP16\age-gender-recognition-retail-0013.xml"),
            ("age-gender-recognition-retail-0013 [FP32-INT8]", @"models\SYNNEX_demo\intel\age-gender-recognition-retail-0013\FP32-INT8\age-gender-recognition-retail-0013.xml")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model2_list = new List<(string Model_Name, string Model_Path)>
        {
            ("head-pose-estimation-adas-0001 [FP32]", @"models\SYNNEX_demo\intel\head-pose-estimation-adas-0001\FP32\head-pose-estimation-adas-0001.xml"),
            ("head-pose-estimation-adas-0001 [FP16]", @"models\SYNNEX_demo\intel\head-pose-estimation-adas-0001\FP16\head-pose-estimation-adas-0001.xml"),
            ("head-pose-estimation-adas-0001 [FP32-INT8]", @"models\SYNNEX_demo\intel\head-pose-estimation-adas-0001\FP32-INT8\head-pose-estimation-adas-0001.xml")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model3_list = new List<(string Model_Name, string Model_Path)>
        {
            ("emotions-recognition-retail-0003 [FP32]", @"models\SYNNEX_demo\intel\emotions-recognition-retail-0003\FP32\emotions-recognition-retail-0003.xml"),
            ("emotions-recognition-retail-0003 [FP16]", @"models\SYNNEX_demo\intel\emotions-recognition-retail-0003\FP16\emotions-recognition-retail-0003.xml"),
            ("emotions-recognition-retail-0003 [FP32-INT8]", @"models\SYNNEX_demo\intel\emotions-recognition-retail-0003\FP32-INT8\emotions-recognition-retail-0003.xml")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model4_list = new List<(string Model_Name, string Model_Path)>
        {
            ("facial-landmarks-35-adas-0002 [FP32]", @"models\SYNNEX_demo\intel\facial-landmarks-35-adas-0002\FP32\facial-landmarks-35-adas-0002.xml"),
            ("facial-landmarks-35-adas-0002 [FP16]", @"models\SYNNEX_demo\intel\facial-landmarks-35-adas-0002\FP16\facial-landmarks-35-adas-0002.xml"),
            ("facial-landmarks-35-adas-0002 [FP32-INT8]", @"models\SYNNEX_demo\intel\facial-landmarks-35-adas-0002\FP32-INT8\facial-landmarks-35-adas-0002.xml")
        };
        */
        string[] Target_device_list = new string[] { "CPU", "GPU", "MYRIAD" };


        #region CameraPreview
        MediaCapture mediaCapture;
        bool isPreviewing,cam_init = false, previewing = false;
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


        public Interactive_Face_Detection_Demo_Page()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Application_Suspending;

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
            foreach (model_dataList model_DataList in model4_list)
            {
                foreach (string precision in model_DataList.Model_precision_Support)
                {
                    model4_name.Items.Add(model_DataList.Model_Name + " [" + precision + "]");
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
                                Parameter += " -m_ag " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_ag " + model1_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_ag " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
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
                                Parameter += " -m_em " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_em " + model3_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_em " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }
            if (model4_name.SelectedItem == null)
            {
                MessageDialog messageDialogs = new MessageDialog("You have to " + model4_TextBlock.Text, "ERROR!");
                messageDialogs.ShowAsync();
                return;
            }
            else
            {
                foreach (model_dataList model_DataList in model4_list)
                {
                    foreach (string precision in model_DataList.Model_precision_Support)
                    {
                        if (model4_name.SelectedItem.ToString().Contains(precision) && model4_name.SelectedItem.ToString().Contains(model_DataList.Model_Name))
                        {

                            if (model4_target.SelectedItem != null)
                                Parameter += " -m_lm " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml -d_lm " + model4_target.SelectedItem.ToString() + " ";
                            else
                                Parameter += " -m_lm " + Model_rootPath + model_DataList.Model_Path + model_DataList.Model_Name + @"\" + precision + @"\" + model_DataList.Model_Name + ".xml ";
                        }
                    }
                }
            }

            // Send Request to ConsoleConnector
            await ((App)Application.Current).SendRequestToConsoleConnector("Interactive_face_detection_demo", Parameter + " -i cam");
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
            
            if(!previewing)
            {
                PreviewControl.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();
                previewing = true;
            }
            
            //await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }
    }
        
}
