using Microsoft.VisualBasic;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    public class State
    {
        public bool RunState { get; set; }
        public long Position { get; set; }
    }
   
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        BackgroundWorker BackgroundWorker { get; set; }
        State playingState = new();
        WaveOutEvent outputDevice = new();

        public Home()
        {
            InitializeComponent();
            playingState.RunState = false;
            playingState.Position = 0;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            Volume.Value = 20;
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.DoWork += testPlay;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Cleanup();
                PerformLogout();
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

        private void playButton_Click(object? sender, RoutedEventArgs e)
        {
            if (playingState.RunState is true) //SONG IS NOT PLAYING
            {
                playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
                playingState.RunState = false;
            }
            else //SONG IS PLAYING
            {
                playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "pause-button.png", UriKind.Relative));
                playingState.RunState = true;

                if(outputDevice.PlaybackState == PlaybackState.Stopped)
                {
                    BackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void testPlay(object? sender, DoWorkEventArgs args)
        {
            using (var audioFile = new AudioFileReader(ClientConstants.Mp3Directory + "mymp3.mp3.mp3"))
            {
                TimeSpan tt = audioFile.TotalTime;

                Dispatcher.Invoke(() =>
                {
                    progressBar.Maximum = audioFile.Length;

                    endTime.Content = string.Format("{0:D2}:{1:D2}",
                        tt.Minutes,
                        tt.Seconds);

                    outputDevice.Volume = (float)Volume.Value / 100;
                });

                audioFile.Position = playingState.Position;

                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Dispatcher.Invoke((Delegate)(() =>
                    {
                        if(outputDevice.PlaybackState is PlaybackState.Playing)
                        {
                            progressBar.Value = playingState.Position = outputDevice.GetPosition();
                            TimeSpan t = outputDevice.GetPositionTimeSpan();
                            startTime.Content = string.Format("{0:D2}:{1:D2}",
                                                t.Minutes,
                                                t.Seconds);
                            Volume.Value = (double)outputDevice.Volume * 100;
                        }
                            
                    }));

                    if (playingState.RunState is false)
                    {
                        if (!(outputDevice.PlaybackState == PlaybackState.Stopped))
                        {
                            outputDevice.Pause();
                            while (playingState.RunState is false)
                            {
                            }

                            outputDevice.Play();
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                    
                Dispatcher.Invoke(() =>
                {
                    playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
                    progressBar.Value += (audioFile.Position - progressBar.Value);
                });

                Cleanup();
            }
        }

        
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float vol = (float)Volume.Value / 100;
            outputDevice.Volume = vol;

            Volume.Value = (double)outputDevice.Volume * 100;
        }

        private void Cleanup()
        {
            BackgroundWorker.CancelAsync();
            BackgroundWorker.Dispose();

            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
            }

            playingState.RunState = false;
            playingState.Position = 0;
        }

        private void PerformLogout()
        {
            Login newWindow = new Login();
            newWindow.Show();
            Close();
        }
    }
}
