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
        private readonly List<(string Model_Name,string Model_Path)> _model0_list = new List<(string Model_Name, string Model_Path)>
        {
            ("model0-1","model0-path01")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model1_list = new List<(string Model_Name, string Model_Path)>
        {
            ("model0-1","model0-path01")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model2_list = new List<(string Model_Name, string Model_Path)>
        {
            ("model0-1","model0-path01")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model3_list = new List<(string Model_Name, string Model_Path)>
        {
            ("model0-1","model0-path01")
        };
        private readonly List<(string Model_Name, string Model_Path)> _model4_list = new List<(string Model_Name, string Model_Path)>
        {
            ("model0-1","model0-path01")
        };
        string[] Target_device_list = new string[] { "CPU", "GPU", "MYRIAD" };

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

        public Interactive_Face_Detection_Demo_Page()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Application_Suspending;

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string parameter_string;
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFolder storageFolder = Windows.Storage.KnownFolders.DocumentsLibrary;
            Windows.Storage.StorageFile ParameterFile =
                await storageFolder.CreateFileAsync("Parameter_to_connector.inf",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            parameter_string = " -m \"" + model0_name.Text + "\" ";
            parameter_string += " -i cam ";
            
            await Windows.Storage.FileIO.WriteTextAsync(ParameterFile, parameter_string);


#if (DEBUG)
            
            var messageDialog = new MessageDialog("[ DEBUG ] Parameter: " + parameter_string + "\n Save as " + storageFolder.Path);
            await messageDialog.ShowAsync();
#endif

            if (cam_init && previewing)
            {
                await mediaCaptureMgr.StopPreviewAsync();
                await CleanupCameraAsync();
                previewing = false;
                
            }
            

            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("interactive_face_detection_demo");
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
