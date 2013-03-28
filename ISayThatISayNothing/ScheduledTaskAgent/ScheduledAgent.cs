using ISayThatISayNothing.API;
using ISayThatISayNothing.Models;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ScheduledTaskAgent
{
    public class ScheduledAgent : Microsoft.Phone.Scheduler.ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
#if DEBUG
            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = "ISayThatISayNothing";
            toast.Content = "Background agent working...";
            toast.Show();
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            WebApi.GetPaginatedMessages(1, (List<MessageModel> recent) =>
            {
                if (recent != null && recent.Count > 0)
                {
                    SetTileData(recent[0]);
                }
            });
            
            NotifyComplete();
        }

        private void SetTileData(MessageModel message)
        {
            var MainTile = ShellTile.ActiveTiles.First();
            if (MainTile != null)
            {
				MainTile.Update(new StandardTileData() { BackContent = message.message, BackTitle = message.author });
            }
        }
    }
}