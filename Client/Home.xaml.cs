using NAudio.Gui;
using NAudio.Utils;
using NAudio.Wave;
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
        public long Position { get; set; }
    }
   
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        Thread playingThread;
        State playing = new();
        WaveOutEvent outputDevice = new();

        public Home()
        {
            InitializeComponent();
            playing.RunState = false;
            playing.Position = 0;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Login newWindow = new Login();
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
            }
            else //SONG IS PLAYING
            {
                playImg.ImageSource = new BitmapImage(new Uri("pause-button.png", UriKind.Relative));
                playing.RunState = true;

                if(outputDevice.PlaybackState == PlaybackState.Stopped)
                {
                    playingThread = new Thread(() => testPlay("mymp3.mp3.mp3", playing));
                    playingThread.Start();
                }
            }
        }

        private void testPlay(string songName, State playingState)
        {
            using (var audioFile = new AudioFileReader(songName))
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
                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = playingState.Position = outputDevice.GetPosition();
                        TimeSpan t = outputDevice.GetPositionTimeSpan();
                        startTime.Content = string.Format("{0:D2}:{1:D2}",
                                            t.Minutes,
                                            t.Seconds);
                        Volume.Value = (double)outputDevice.Volume * 100;
                    });

                    if (playingState.RunState is false)
                    {
                        if(!(outputDevice.PlaybackState == PlaybackState.Stopped))
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
                    playImg.ImageSource = new BitmapImage(new Uri("play-button.png", UriKind.Relative));
                    progressBar.Value += (audioFile.Position - progressBar.Value);
                });


                outputDevice.Stop();
                outputDevice.Dispose();
                playing.RunState = false;
                playing.Position = 0;
            }
            
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float vol = (float)Volume.Value / 100;
            outputDevice.Volume = vol;

            Volume.Value = (double)outputDevice.Volume * 100;
        }
    }
}
