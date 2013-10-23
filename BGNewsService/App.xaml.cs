using BGNewsService.Common;
using BGNewsService.DataModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Callisto.Controls;
using Windows.UI;
using BGNewsService.Flyouts;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.Background;

namespace BGNewsService
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Task name.
        /// </summary>
        private const string taskName = "NewsFeedBackgroundTask";

        /// <summary>
        /// Task entry point for the tile notifications.
        /// </summary>
        private const string taskEntryPoint = "BackgroundTasks.NewsFeedBackgroundTask";

        /// <summary>
        /// The foreground color of the privacey policy flyout header.
        /// </summary>
        private Color headerBrush =  Color.FromArgb(46, 23, 0, 0);

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!IsConnectedToInternet())
            {
               await AllNewsGroupsViewModel.PopulateGroupsFromLocalFolder();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            this.RegisterBackgroundTask();

            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

            Window.Current.VisibilityChanged += CurrentVisibilityChanged;
            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }

                }
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            if (!IsConnectedToInternet())
            {
                MessageDialog md = new MessageDialog("Your device is not connected to the internet!");
                await md.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Command for the settings flyout.
        /// </summary>
        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var about = new SettingsCommand("privacy policy", "Privacy Policy", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.Content = new PrivacyPolicyPage();
                settings.HeaderBrush = new SolidColorBrush(headerBrush);
                settings.HeaderText ="Privacy Policy";
                settings.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(about);
        }  

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                BGNewsService.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await BGNewsService.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (BGNewsService.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(Views.SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        ///// <summary>
        ///// Invoked when the visibility of the current page is changed.
        ///// </summary>
        Task currentSaveTask;
        private void CurrentVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible && currentSaveTask == null)
            {
               currentSaveTask = SaveData();
            }
        }

        /// <summary>
        /// Saves the data from all news groups in local folder.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task SaveData()
        {
            bool isConnected = IsConnectedToInternet();
            if (isConnected)
            {
                var groups = AllNewsGroupsViewModel.GetGroups("AllGroups");
                await DataSaverAndLoader.SaveDataToFiles(groups);
            }
            currentSaveTask = null;
        }

        /// <summary>
        /// Checks if the device is connected to the internet.
        /// </summary>
        /// <returns></returns>
        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            bool isConnected = (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel()
                == NetworkConnectivityLevel.InternetAccess);
            return isConnected;
        }

        /// <summary>
        /// Background task for tile notifications.
        /// </summary>
        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }
    }
}