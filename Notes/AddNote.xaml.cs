using Notes.Data_Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Notes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNote : Page
    {
        private bool isViewing = false;
        private Note myNote;
        public AddNote()
        {
            this.InitializeComponent();
        }

       
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Geopoint mypoint;
            if (e.Parameter == null)
            {
                // this is when the user is adding
                isViewing = false;
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;

                var position = await locator.GetGeopositionAsync();
                mypoint = position.Coordinate.Point;

            }
            else
            {
                // when the user is viewing/ deleting
                isViewing = true;
                 myNote = (Note)e.Parameter;
                titleTextBox.Text = myNote.Title;
                noteTextBox.Text = myNote.Notes;
                addButton.Content = "Delete";

                var myPosition = new Windows.Devices.Geolocation.BasicGeoposition();
                myPosition.Latitude = myNote.Latitude;
                myPosition.Longitude = myNote.Longitude;

                mypoint = new Geopoint(myPosition);

            }
            await MyMap.TrySetViewAsync(mypoint, 19D);
        }
       private async void addButton_Click(object sender, RoutedEventArgs e)
          {

            if (isViewing)
            {
                // Deleting message box

                var messageDialog = new Windows.UI.Popups.MessageDialog("Do you want to delete Note?");
                messageDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));

                // enable the command to be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                //set command to be invoked when escape is pressed.
                messageDialog.CancelCommandIndex = 1;

                await messageDialog.ShowAsync();


                // enable delete option
                App.DataModel.DeleteMapNote(myNote);
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                // enable adding option
                Note newNote = new Note();
                newNote.Title = titleTextBox.Text;
                newNote.Notes = noteTextBox.Text;
                newNote.DateCreated = DateTime.Now;
                newNote.Longitude = MyMap.Center.Position.Longitude;
                newNote.Latitude = MyMap.Center.Position.Latitude;

                App.DataModel.AddNote(newNote);
                Frame.Navigate(typeof(MainPage));
            }
            Frame.Navigate(typeof(MainPage));
        }
            private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }
        private void CommandInvokedHandler(IUICommand  command)
{
if  (command.Label == "Delete")
{
App.DataModel.DeleteMapNote(myNote);
Frame.Navigate(typeof(MainPage));
}
}

       
    }
}
