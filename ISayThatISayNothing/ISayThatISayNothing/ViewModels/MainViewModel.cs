using ISayThatISayNothing.API;
using ISayThatISayNothing.Models;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Data;


namespace ISayThatISayNothing
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string setLastDBUpdateKey = "LastDBUpdate";
        private PeriodicTask taskRefreshTileData;

        public ObservableCollection<MessageModel> Items { get; private set; }
        public CollectionViewSource TopItems { get; private set; }
        public CollectionViewSource FlopItems { get; private set; }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<MessageModel>();
            this.TopItems = new CollectionViewSource() { Source = this.Items };
            this.TopItems.SortDescriptions.Add(new SortDescription("nbTop", ListSortDirection.Descending));
            this.FlopItems = new CollectionViewSource() { Source = this.Items };
            this.FlopItems.SortDescriptions.Add(new SortDescription("nbFlop", ListSortDirection.Descending));

            StartPeriodicAgent();
        }   

        private bool _IsDataLoaded = false;
        public bool IsDataLoaded
        {
            get
            {
                return _IsDataLoaded;
            }
            private set
            {
                if (value != _IsDataLoaded)
                {
                    _IsDataLoaded = value;
                    NotifyPropertyChanged("IsDataLoaded");
                }
            }
        }

        public void LoadData()
        {
            IsDataLoaded = false;
            WebApi.GetAllMessages(OnDataLoaded);
            //WebApi.GetMessagesSince(new DateTime(2012, 11, 27), OnDataLoaded);
        }

        private void OnDataLoaded(List<MessageModel> messages)
        {
            this.Items.Clear();
            foreach (MessageModel mm in messages)
            {
                this.Items.Add(mm);
            }

            // Store the last update time
            IsolatedStorageSettings.ApplicationSettings[setLastDBUpdateKey] = DateTime.Now;
            IsolatedStorageSettings.ApplicationSettings.Save();

            // Update the tile with the last message seen
            if(messages[0] != null) SetTileData(messages[0]);

            this.IsDataLoaded = true;
        }

        private void SetTileData(MessageModel message)
        {
            var MainTile = ShellTile.ActiveTiles.First();
            if (MainTile != null)
            {
                MainTile.Update(new StandardTileData() { BackContent = message.message, BackTitle = message.author });
            }
        }

        private void StartPeriodicAgent()
        {
            // Only start the agent if the main tile exists
            var MainTile = ShellTile.ActiveTiles.First();
            if (MainTile != null)
            {
                string periodicTaskName = "istisnTileTask";

                // Obtain a reference to the period task, if one exists
                taskRefreshTileData = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

                // If the task already exists and background agents are enabled for the
                // application, you must remove the task and then add it again to update 
                // the schedule
                if (taskRefreshTileData != null)
                {
                    try
                    {
                        ScheduledActionService.Remove(periodicTaskName);
                    }
                    catch (Exception)
                    {
                    }
                }

                taskRefreshTileData = new PeriodicTask(periodicTaskName);

                // The description is required for periodic agents. This is the string that the user
                // will see in the background services Settings page on the device.
                taskRefreshTileData.Description = "Updates the application's tile with the latest quotes.";

                // Place the call to Add in a try block in case the user has disabled agents.
                try
                {
                    ScheduledActionService.Add(taskRefreshTileData);
#if DEBUG
                    ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
                    Debug.WriteLine("PeriodicTask : Tile Updated has been started.");
                }
                catch (InvalidOperationException exception)
                {
                    Debug.WriteLine(exception.Message);
                }
                catch (SchedulerServiceException)
                {
                    // No user action required.
                }
            }
            else
            {
                Debug.WriteLine("Main tile not found : skipping the Tile Updater init.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}