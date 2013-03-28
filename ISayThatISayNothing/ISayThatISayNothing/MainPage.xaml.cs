using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ISayThatISayNothing
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            SystemTray.SetProgressIndicator(this, new ProgressIndicator() { IsIndeterminate = true });
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            App.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = App.ViewModel;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDataLoaded")
            {
                if (App.ViewModel.IsDataLoaded == true)
                {
                    LoadingEndedAnimation.Begin();
                    SystemTray.ProgressIndicator.IsVisible = false;
                }
                else
                {
                    LoadingAnimation.Begin();
                    SystemTray.ProgressIndicator.IsVisible = true;
                }
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
                LoadingAnimation.Begin();
            }
        }

        private void AppBarAbout_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

		private void LeFrancais_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			App.ViewModel.LoadData(); // refresh data !
		}
    }
}