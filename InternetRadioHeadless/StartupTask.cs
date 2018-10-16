using InternetRadioShared.Models;
using InternetRadioShared;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;

// The Background Application template is documented at Socket://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace InternetRadioHeadless
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private SocketServer socketServer;

        #region IBackgroundTask and IBackgroundTaskInstance Interface Members and handlers

        /// <summary>
        /// The Run method is the entry point of a background task. 
        /// </summary>
        /// <param name="taskInstance"></param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                // This must be retrieved prior to subscribing to events below which use it
                deferral = taskInstance.GetDeferral();

                // Start socket server
                socketServer = new SocketServer();
                socketServer.Start(Constants.Port);

                // Associate a cancellation and completed handlers with the background task.
                taskInstance.Task.Completed += TaskCompleted;
                // Event may raise immediately before continung thread excecution so must be at the end
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            }
            catch { }
        }

        /// <summary>
        /// Indicate that the background task is completed.
        /// </summary>       
        void TaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            deferral.Complete();
        }

        /// <summary>
        /// Handles background task cancellation. Task cancellation happens due to:
        /// 1. Another Media app comes into foreground and starts playing music 
        /// 2. Resource pressure. Your task is consuming more CPU and memory than allowed.
        /// In either case, save state so that if foreground app resumes it can know where to start.
        /// </summary>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Shutdown media pipeline
            try { BackgroundMediaPlayer.Shutdown(); } catch { }

            // Signals task completition
            deferral.Complete();
        }

        #endregion
    }
}
