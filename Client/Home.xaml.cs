using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    public class State
    {
        public bool RunState { get; set; }
    }
   
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        Thread playingThread;
        State playing = new();

        public Home()
        {
            InitializeComponent();
            playing.RunState = false;
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow newWindow = new MainWindow();
                newWindow.Show();
                this.Close();
            }
            
        }

        private void searchtb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // user hasn't entered a username;
            if (tb.Text == string.Empty)
            {
                tb.Text = "Search...";
            }
        }

        private void searchtb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // username is placeholder
            if (tb.Text == "Search...")
            {
                tb.Text = string.Empty;
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void volumeUp_Click(object sender, RoutedEventArgs e)
        {
            if (Volume.Value != 100)
            {
                Volume.Value++;
            }
        }

        private void volumeDown_Click(object sender, RoutedEventArgs e)
        {
            if (Volume.Value != 0)
            {
                Volume.Value--;
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (playing.RunState is true) //SONG IS NOT PLAYING
            {
                playImg.ImageSource = new BitmapImage(new Uri("play-button.png", UriKind.Relative));
                playing.RunState = false;
                playingThread.Join();
            }
            else //SONG IS PLAYING
            {
                playImg.ImageSource = new BitmapImage(new Uri("pause-button.png", UriKind.Relative));
                playing.RunState = true;

                playingThread = new Thread(() => testPlay(playing));
                playingThread.Start();
            }
        }

        private void testPlay(State playingState)
        {
            while(playingState.RunState == true)
            {
                Dispatcher.Invoke(() =>
                {
                    progressBar.Value += 0.1;
                });

                Thread.Sleep(1000);
            }
        }
    }
}
