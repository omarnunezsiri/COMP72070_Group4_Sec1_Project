using Microsoft.VisualBasic;
using NAudio.Utils;
using NAudio.Wave;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

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
        //list of song names to pretend to select from as if it was a database of songs                 DELETE THIS
        public List<Song> fakeResults { get; set; } = new List<Song>
        {
            new Song("Anti-Hero", "No idea", "Taylor Swift", 100),
            new Song("Hello", "No glue", "Adele", 200),
            new Song("Golden Hour", "Shrug", "JVKE", 300),
        };

        BackgroundWorker BackgroundWorker { get; set; }
        State playingState = new();
        WaveOutEvent outputDevice = new();

        //search stuff
        //this list "searchResults" should be populated with the search results from the server
        public List<Song> searchResults { get; set; } = new List<Song> { };

        List<Grid> buttonList = new List<Grid>();   //list for storing search result buttons

        //next up is a bunch of trash that defines the size and where the buttons appear


        int currentY = 0; //Y location of the current button
        int Xloc = 0; //X location to put every button

        //button dimensions
        const int WIDTH = 400;
        const int HEIGHT = 60;

        //is the search view currently showing
        bool searchActive = false;

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

        //search stuff
        /// <summary>
        /// CURRENTLY UNUSED Clears search results
        /// </summary>
        public void clearSearch()
        {
            if (searchActive)
            {
                foreach (Grid button in buttonList)
                {
                    MainCanvas.Children.Remove(button);
                }
                searchActive = false;
                currentY = 64;

                searchResults.Clear();
            }
        }

        /// <summary>
        /// stubbed search function, will pick 5 randoms names from 'fakeResults' NEED TO REPLACE WITH MAKING SEARCH REQUEST
        /// </summary>
        /// <param name="resultList"></param>
        public void search(List<Song> resultList)
        {
            PacketHeader searchHeader = new(PacketHeader.SongAction.List);
            SearchBody searchBody = new(0x00000000, searchtb.Text);

            Packet searchPacket = new(searchHeader, searchBody);

            byte[] TxBuffer = searchPacket.Serialize();

            // Send TxBuffer


            //EVERYTHING IN HERE NEEDS TO BE REPLACED WITH DATA COMMS CRAP
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                int index = r.Next(3);
                resultList.Add(fakeResults[index]);
            }
        }

        /// <summary>
        /// creates a new button for the user to see and click on (THIS COULD PROBABLY BE BROKEN DOWN INTO MORE OR SOMETING
        /// </summary>
        private void MakeButton()
        {
            buttonList.Clear();
            search(searchResults);      //this makes the search results. Change this to be passed in

            for (int i = 0; i < searchResults.Count; i++)
            {
                //create the grid container for the search item
                Grid newGrid = new Grid();
                newGrid.Style = (Style)FindResource("ResultGrid");
                newGrid.Name = "item" + i;

                //oh god here we go. Create all the rows and columns
                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(60);
                ColumnDefinition col2 = new ColumnDefinition();
                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                newGrid.ColumnDefinitions.Add(col1);
                newGrid.ColumnDefinitions.Add(col2);
                newGrid.RowDefinitions.Add(row1);
                newGrid.RowDefinitions.Add(row2);
                newGrid.MouseDown += SelectSong;


                ////create the album image
                //System.Windows.Controls.Image albumCover = new Image();
                //albumCover.Style = (Style)FindResource("AlbumImage");

                ////jesus christ giant bitmap thing that will probably be replaced anyways
                //FileStream fs = File.Open("Tmp/" + searchResults[i].Name + ".png", FileMode.Open);
                //System.Drawing.Bitmap dImg = new System.Drawing.Bitmap(fs);
                //MemoryStream ms = new MemoryStream();
                //dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                //bImg.BeginInit();
                //bImg.StreamSource = new MemoryStream(ms.ToArray());
                //bImg.EndInit();

                //albumCover.Source = bImg;

                //fs.Close();


                //create the song name text
                TextBlock songName = new TextBlock();
                songName.Style = (Style)FindResource("SongName");
                songName.Text = searchResults[i].GetName();

                //Create artist text block
                TextBlock artistName = new TextBlock();
                artistName.Style = (Style)FindResource("ArtistName");
                artistName.Text = searchResults[i].GetArtist();

                //add all elements to grid
                //newGrid.Children.Add(albumCover);             //TEMP COMMENTING OUT, NEED TO FIGURE OUT WHERE THESE IMAGES ARE GETTING PUT
                newGrid.Children.Add(songName);
                newGrid.Children.Add(artistName);

                buttonList.Add(newGrid);
                //Button newBtn = new Button();
                //newBtn.Content = searchResults[i];
                //newBtn.Name = "button" + i.ToString();
                //newBtn.Width = WIDTH;
                //newBtn.Height = HEIGHT;
                //newBtn.Click += NewBtn_Click;
                //buttonList.Add(newBtn);
            }
        }

        /// <summary>
        /// Selects song to play                NEED TO 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSong(object sender, MouseButtonEventArgs e)
        {
            Grid clickedGrid = (Grid)sender;
            string clickedItem = clickedGrid.Name.Remove(0, 4);
            Debug.WriteLine(clickedItem);
            //clearSearch();
            int i = Int32.Parse(clickedItem);

            
            Console.WriteLine(searchResults[i].ToString());  //this is supposed to be the request to stream/play the song. Replace with requesting song from server
        }

        /// <summary>
        /// when clicking enter on search bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!searchActive)
                {
                    MakeButton();
                    foreach (Grid button in buttonList)
                    {
                        Canvas.SetTop(button, currentY);
                        Canvas.SetLeft(button, Xloc);
                        currentY += HEIGHT;
                        MainCanvas.Children.Add(button);    //adds the button to the canvas
                    }
                    searchActive = true;
                }
            }
        }
    }
}
