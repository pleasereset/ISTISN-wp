using ISayThatISayNothing.API;
using ISayThatISayNothing.Models;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Data;


namespace ISayThatISayNothing
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string setLastDBUpdateKey = "LastDBUpdate";

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