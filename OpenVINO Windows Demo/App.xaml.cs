using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace OpenVINO_Windows_Demo
{
    /// <summary>
    /// 提供應用程式專屬行為以補充預設的應用程式類別。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化單一應用程式物件。這是第一行執行之撰寫程式碼，
        /// 而且其邏輯相當於 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在應用程式由終端使用者正常啟動時叫用。當啟動應用
        /// 將在例如啟動應用程式時使用以開啟特定檔案。
        /// </summary>
        /// <param name="e">關於啟動要求和處理序的詳細資料。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            APPLaunch();

            Frame rootFrame = Window.Current.Content as Frame;

            // 當視窗中已有內容時，不重複應用程式初始化，
            // 只確定視窗是作用中
            if (rootFrame == null)
            {
                // 建立框架做為巡覽內容，並巡覽至第一頁
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 從之前暫停的應用程式載入狀態
                }

                // 將框架放在目前視窗中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 在巡覽堆疊未還原時，巡覽至第一頁，
                    // 設定新的頁面，方式是透過傳遞必要資訊做為巡覽
                    // 參數
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 確定目前視窗是作用中
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 在巡覽至某頁面失敗時叫用
        /// </summary>
        /// <param name="sender">導致巡覽失敗的框架</param>
        /// <param name="e">有關巡覽失敗的詳細資料</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在應用程式暫停執行時叫用。應用程式狀態會儲存起來，
        /// 但不知道應用程式即將結束或繼續，而且仍將記憶體
        /// 的內容保持不變。
        /// </summary>
        /// <param name="sender">暫停之要求的來源。</param>
        /// <param name="e">有關暫停之要求的詳細資料。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 儲存應用程式狀態，並停止任何背景活動
            try
            {
                SendRequestToConsoleConnector("Command", "Close");
            }
            catch(Exception)
            {

            }
            
            deferral.Complete();
        }
        


        AppServiceConnection Connection = null;
        BackgroundTaskDeferral appServiceDeferral = null;
        public async void APPLaunch()
        {
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }
        /// <summary>
        /// Override the Application.OnBackgroundActivated method to handle background activation in 
        /// the main process. This entry point is used when BackgroundTaskBuilder.TaskEntryPoint is 
        /// not set during background task registration.
        /// </summary>
        /// <param name="args"></param>
        /// 
        /// <summary>
        /// Initializes the app service on the host process 
        /// </summary>
        protected async override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {

            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
            {

                appServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += OnTaskCanceled; // Associate a cancellation handler with the background task.

                AppServiceTriggerDetails details = args.TaskInstance.TriggerDetails as AppServiceTriggerDetails;
                Connection = details.AppServiceConnection;

                // Send request to ConsoleConnector
                try
                {
                    await SendRequestToConsoleConnector("Command", "initialize");
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// Associate the cancellation handler with the background task 
        /// </summary>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.appServiceDeferral != null)
            {
                // Complete the service deferral.
                this.appServiceDeferral.Complete();
            }
        }
        public async Task SendRequestToConsoleConnector(string requestKay_str,string request_str)
        {

            ValueSet request = new ValueSet();
            request.Add(requestKay_str, request_str);


            AppServiceResponse response = null;
            response = await Connection.SendMessageAsync(request);
            if(response.Message.Keys.Contains("CPU"))
            {
                string CPU_info = response.Message["CPU"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["CPU"] = CPU_info;
            }
            else if (response.Message.Keys.Contains("OpenVINO"))
            {
                string OpenVINO_info = response.Message["OpenVINO"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["OpenVINO"] = OpenVINO_info;
            }
            else if (response.Message.Keys.Contains("SampleDemo"))
            {
                string OpenVINO_info = response.Message["SampleDemo"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["SampleDemo"] = OpenVINO_info;
            }
            else if (response.Message.Keys.Contains("OMZ_Model"))
            {
                string OpenVINO_info = response.Message["OMZ_Model"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["OMZ_Model"] = OpenVINO_info;
            }
            else if (response.Message.Keys.Contains("All_Model_Name"))
            {
                string OpenVINO_info = response.Message["All_Model_Name"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["All_Model_Name"] = OpenVINO_info;
            }
            else if (response.Message.Keys.Contains("Get_All_Model_info_json"))
            {
                string OpenVINO_info = response.Message["Get_All_Model_info_json"] as string;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["Get_All_Model_info_json"] = OpenVINO_info;
            }
            else
            {
                
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["OMZ_Model"] = "No Detect";
            }
        }
    }
}

