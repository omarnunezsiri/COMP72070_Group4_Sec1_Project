﻿using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Printing;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;

namespace Client
{ 
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        /* Media player members */
        WaveOutEvent outputDevice;
        AudioFileReader audioFileReader;

        /* Information about the current song being played */
        string currentSongLocation;
        string currentSongName;
        bool isNewAudioFile;

        /* Used for asynchronous programming */
        CancellationTokenSource cancellationTokenSource;
        Task playTask;

        /* Data Communications */
        byte[] TxBuffer;
        byte[] RxBuffer;
        MediaControlBody.State state;

        string mp3Dir;

        //search stuff
        //this list "searchResults" should be populated with the search results from the server
        public List<Song> searchResults { get; set; } = new List<Song> { };

        List<Grid> buttonList = new List<Grid>();   //list for storing search result buttons

        int currentY = 0; //Y location of the current button
        int Xloc = 0; //X location to put every button

        //button dimensions
        const int WIDTH = 400;
        const int HEIGHT = 40;

        //is the search view currently showing
        bool searchActive = false;

        bool repeatActive = false;
        public Home(string username)
        {
            InitializeComponent();
            outputDevice = new();

            progressBar.Minimum = 0;
            progressBar.Value = 0;
            Volume.Value = Constants.DefaultVolume;
            state = MediaControlBody.State.Idle;
            currentSongLocation = "";
            currentSongName = "";
            RxBuffer = new byte[Constants.Mp3BufferMax + Constants.CoverBufferMax];
            cancellationTokenSource = new CancellationTokenSource();
            outputDevice = new();

            /* Determines if the current user has its own dedicted Mp3s folder */
            mp3Dir = ClientConstants.AssetsDirectory + $"{username}Mp3/";
            if(!Directory.Exists(mp3Dir))
            {
                Directory.CreateDirectory(mp3Dir);
            }
        }


        /*********** MEDIA ***********/

        /// <summary>
        /// Async task in charge of playing a song and performing UI-related changes
        /// </summary>
        /// <param name="songLocation">path to the .mp3 file to play</param>
        /// <param name="cancellationToken">cancellation token for propen synchronization</param>
        /// <returns>The PlayMp3Async task</returns>
        private async Task PlayMp3Async(string songLocation, CancellationToken cancellationToken)
        {
            if(audioFileReader is null || isNewAudioFile)
            {
                /* Initial Song Setup...Labels, progress bar max and audio load */

                isNewAudioFile = false;
                audioFileReader = new(songLocation);
                TimeSpan totalDuration = audioFileReader.TotalTime;

                await Dispatcher.InvokeAsync(() =>
                {
                    progressBar.Maximum = audioFileReader.Length;

                    endTime.Content = string.Format("{0:D2}:{1:D2}",
                        totalDuration.Minutes,
                        totalDuration.Seconds);

                    outputDevice.Volume = (float)Volume.Value / 100;
                });

                outputDevice.Init(audioFileReader);
            }

            /* Keeps looping until the song ends or a cancellation is requested through the cancellation token */
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing && !cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    /* Tell the dispatcher to update UI information for formatting */
                    if (outputDevice.PlaybackState is PlaybackState.Playing)
                    {
                        progressBar.Value = audioFileReader.Position;
                        TimeSpan currentPosition = audioFileReader.CurrentTime;
                        startTime.Content = string.Format("{0:D2}:{1:D2}",
                                            currentPosition.Minutes,
                                            currentPosition.Seconds);
                        Volume.Value = (double)outputDevice.Volume * 100;
                    }
                });

                await Task.Delay(100);
            }

            /* If the song ended, perform a cleanup. */
            if(SongReachedEnd())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressBar.Value += (audioFileReader.Position - progressBar.Value);
                    PlayCleanup();
                });

                if(repeatActive)
                {
                    if(!cancellationToken.IsCancellationRequested)
                    {
                        playTask = PlayMp3Async(songLocation, cancellationToken);
                    }
                }
                else
                {
                    /* Handshake between Client and Server to end media play */
                    await Dispatcher.InvokeAsync(EndPlayCommunication);
                }
            } 
        }

        private bool SongReachedEnd()
        {
            return (outputDevice.PlaybackState == PlaybackState.Stopped);
        }

        private void PlayCleanup()
        {
            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                outputDevice.Stop();
            }

            outputDevice.Dispose();

            if(repeatActive is false || playTask.IsCompleted)
            {
                playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
                state = MediaControlBody.State.Idle;
            }
            
            isNewAudioFile = true;
        }

        /// <summary>
        /// Selects song to play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SelectSong(object sender, MouseButtonEventArgs e)
        {
            Grid clickedGrid = (Grid)sender;
            string clickedItem = clickedGrid.Name.Remove(0, 4);
            int i = Int32.Parse(clickedItem);

            string hash = searchResults[i].GetName();
            string album = searchResults[i].GetAlbum();
            string imagePath = Constants.TempDirectory + $"{searchResults[i].GetAlbum()}.jpg";
            string localMp3Path = mp3Dir + $"{hash}.mp3";

            
            if(currentSongName.Equals(hash)) // checks if the song requested is already being played
            {
                playButton_Click(sender, e);
            }
            else
            {
                if (!File.Exists(localMp3Path))
                {
                    if (outputDevice.PlaybackState != PlaybackState.Stopped)
                    {
                        Directory.GetFiles(Constants.TempDirectory, "*.mp3").ToList()
                                                                            .ForEach(file => File.Delete(file));
                    }

                    PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
                    DownloadBody downloadBody = new(DownloadBody.Type.SongFile, hash);

                    Packet packet = new(packetHeader, downloadBody);

                    TxBuffer = packet.Serialize();

                    App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

                    currentSongLocation = ReceiveDownloadData(hash, DownloadBody.Type.SongFile, true);
                    currentSongName = hash;
                }
                else
                {
                    currentSongLocation = localMp3Path;
                    currentSongName = hash;
                }

                BitmapImage bitmap = LoadRuntimeBitmap(imagePath);
                coverImage.Source = bitmap;
                artist.Text = searchResults[i].GetArtist();
                songName.Text = hash;

                /* Can interrupt an async Task */
                if (playTask is not null && !playTask.IsCompleted)
                {
                    cancellationTokenSource.Cancel();
                    await playTask;
                    state = MediaControlBody.State.Idle;
                }

                if(playTask is not null)
                    PlayCleanup();

                isNewAudioFile = true;
                playButton_Click(sender, e);
            }
        }


        private void EndPlayCommunication()
        {
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Media);
            MediaControlBody mediaControlBody = new MediaControlBody(MediaControlBody.Action.Skip);

            StateTransition(packetHeader, mediaControlBody);
        }

        private void StateTransition(PacketHeader packetHeader, MediaControlBody mediaControlBody)
        {
            Logger instance = Logger.Instance;

            /* Transitions from current <state> to the MediaControlBody.State received from server */
            Packet pausePacket = new(packetHeader, mediaControlBody);

            TxBuffer = pausePacket.Serialize();

            App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);
            instance.Log(pausePacket, true);

            byte[] receivedBuffer = App.client.Receive(ref App.iPEndPoint);

            pausePacket = new(receivedBuffer);
            mediaControlBody = (MediaControlBody)pausePacket.body;

            instance.Log(pausePacket, false);

            state = mediaControlBody.GetState();
        }


        private void ReceiveSongCovers(List<Song> results)
        {
            Logger logger = Logger.Instance;

            foreach (Song song in results)
            {
                string hash = song.GetAlbum();

                PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Download);
                DownloadBody db = new(DownloadBody.Type.AlbumCover, hash);

                Packet packet = new(packetHeader, db);
                TxBuffer = packet.Serialize();
                
                App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);
                logger.Log(packet, true);

                ReceiveDownloadData(hash, db.GetType(), true);
            }
        }

        private string ReceiveDownloadData(string hash, DownloadBody.Type type, bool isTemp)
        {
            /* Determines the path to open the Resource from (AlbumCover or SongFile) */
            string toOpen = "";
            if (type == DownloadBody.Type.AlbumCover)
            {
                toOpen = (isTemp ? Constants.TempDirectory : Constants.ImagesDirectory) + $"{hash}.jpg";
            }
            else if (type == DownloadBody.Type.SongFile)
            {
                toOpen = (isTemp ? Constants.TempDirectory : mp3Dir) + $"{hash}.mp3";
            }


            Logger instance = Logger.Instance;
            bool reachedTotalBlocks = false;
            Packet packet;

            /* Receive each block and write it to the file <toOpen> */
            using (FileStream fs = File.Open(toOpen, FileMode.Create))
            {
                do
                {
                    byte[] receivedBuffer = App.client.Receive(ref App.iPEndPoint);

                    packet = new(receivedBuffer);
                    instance.Log(packet, false);

                    DownloadBody receivedBody = (DownloadBody)packet.body;
                    fs.Write(receivedBody.GetData(), 0, (int)receivedBody.GetDataByteCount());

                    if (!(receivedBody.GetBlockIndex() < receivedBody.GetTotalBlocks() - 1))
                        reachedTotalBlocks = true;

                    App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);
                } while (!reachedTotalBlocks);
            }

            return toOpen;
        }

        private void Skip(TimeSpan timeSpan)
        {
            if (audioFileReader != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                TimeSpan newPosition = audioFileReader.CurrentTime + timeSpan;
                if (newPosition > TimeSpan.Zero && newPosition < audioFileReader.TotalTime)
                {
                    audioFileReader.CurrentTime = newPosition;
                }
            }
        }

        /************ OTHERS ***********/

        private BitmapImage LoadRuntimeBitmap(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            return bitmap;
        }

        private async void PerformLogout()
        {
            if (playTask is not null && !playTask.IsCompleted)
            {
                cancellationTokenSource.Cancel();
                await playTask;
                audioFileReader.Dispose();
                PlayCleanup();
            }

            /* Load login window again */
            Login newWindow = new Login();
            newWindow.Show();
            Close();
        }

        /*********** SEARCH UTILITIES **********/

        //search stuff
        /// <summary>
        /// CURRENTLY UNUSED Clears search results
        /// </summary>
        public void clearSearch()
        {
            if (searchActive)
            {
                Directory.GetFiles(Constants.TempDirectory, "*.jpg").ToList()
                                                    .ForEach(file => File.Delete(file));

                foreach (Grid button in buttonList)
                {
                    MainCanvas.Children.Remove(button);
                }
                searchActive = false;
                currentY = 0;

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

            TxBuffer = searchPacket.Serialize();

            Logger instance = Logger.Instance;
            
            App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);
            instance.Log(searchPacket, true);

            byte[] receivedBuffer = App.client.Receive(ref App.iPEndPoint);

            searchPacket = new(receivedBuffer);
            SearchBody sb = (SearchBody)searchPacket.body;
            instance.Log(searchPacket, false);

            Utils.PopulateSearchResults(sb.GetResponse(), resultList);
            ReceiveSongCovers(resultList);
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
                col1.Width = new GridLength(40);
                ColumnDefinition col2 = new ColumnDefinition();
                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                newGrid.ColumnDefinitions.Add(col1);
                newGrid.ColumnDefinitions.Add(col2);
                newGrid.RowDefinitions.Add(row1);
                newGrid.RowDefinitions.Add(row2);
                newGrid.MouseDown += SelectSong;


                //create the album image
                System.Windows.Controls.Image albumCover = new System.Windows.Controls.Image();
                albumCover.Style = (Style)FindResource("AlbumImage");

                string imagePath = Constants.TempDirectory + $"{searchResults[i].GetAlbum()}.jpg";
                BitmapImage bitmap = LoadRuntimeBitmap(imagePath);
                albumCover.Source = bitmap;


                //create the song name text
                TextBlock songName = new TextBlock();
                songName.Style = (Style)FindResource("SongName");
                songName.Text = searchResults[i].GetName();

                //Create artist text block
                TextBlock artistName = new TextBlock();
                artistName.Style = (Style)FindResource("ArtistName");
                artistName.Text = searchResults[i].GetArtist();

                //Create the download button
                Button downloadButton = new Button();
                downloadButton.Style = (Style)FindResource("DownloadButton");
                downloadButton.Name = newGrid.Name = "item" + i;
                downloadButton.Click += downloadButton_Click;

                if (File.Exists(mp3Dir + $"{searchResults[i].GetName()}.mp3"))
                {
                    ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "delete.png", UriKind.Relative));
                    downloadButton.ToolTip = "Delete";
                    downloadButton.Background = new ImageBrush(bgimg);
                    downloadButton.Width = 16;
                    downloadButton.Height = 16;

                    if (downloadButton.IsMouseOver == true)
                    {
                        downloadButton.Background = new SolidColorBrush(Color.FromRgb(255, 160, 122));
                    }
                }
                else
                {
                    ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "download-button.png", UriKind.Relative));
                    downloadButton.ToolTip = "Download";
                    downloadButton.Background = new ImageBrush(bgimg);
                }

                //add all elements to grid
                newGrid.Children.Add(albumCover);
                newGrid.Children.Add(songName);
                newGrid.Children.Add(artistName);
                newGrid.Children.Add(downloadButton);

                buttonList.Add(newGrid);
            }
        }

        private void PerformSearch()
        {
            noResultstb.Visibility = Visibility.Hidden;
            if (searchActive)
            {
                clearSearch();
                searchActive = false;
            }
            if (!searchActive)
            {
                if (searchtb.Text == string.Empty)
                {
                    clearSearch();
                    noResultstb.Visibility = Visibility.Hidden;
                }
                else
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
            if (searchResults.Count == 0)
            {
                noResultstb.Visibility = Visibility.Visible;
            }
        }


        /********** EVENT HANDLERS **********/

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PlayCleanup();
                PerformLogout();
            }

        }

        private void searchtb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text == string.Empty)
            {
                tb.Text = "Search...";
                clearSearch();
            }
        }

        private void searchtb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text == "Search...")
            {
                tb.Text = string.Empty;
            }
            if (tb.Text == string.Empty)
            {
                clearSearch();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchtb.Text != string.Empty && searchtb.Text != "Search...")
            {
                PerformSearch();
            }

            else if (searchtb.Text == string.Empty)
            {
                clearSearch();
            }
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
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Media);
            MediaControlBody mediaControlBody;
            if (state == MediaControlBody.State.Playing) //SONG IS NOT PLAYING
            {
                outputDevice.Pause();
                mediaControlBody = new(MediaControlBody.Action.Pause);
                StateTransition(packetHeader, mediaControlBody);
                playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
                playButton.ToolTip = "Play";
            }
            else //SONG IS PLAYING
            {
                if (!string.IsNullOrEmpty(currentSongLocation))
                {
                    mediaControlBody = new(MediaControlBody.Action.Play);
                    StateTransition(packetHeader, mediaControlBody);
                    playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "pause-button.png", UriKind.Relative));
                    playButton.ToolTip = "Pause";

                    if (playTask is null || playTask.IsCompleted || playTask.IsCanceled)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        playTask = PlayMp3Async(currentSongLocation, cancellationTokenSource.Token);
                    }
                }
            }
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedGrid = (Button)sender;
            string clickedItem = clickedGrid.Name.Remove(0, 4);
            int i = Int32.Parse(clickedItem);

            String path = mp3Dir + $"{searchResults[i].GetName()}.mp3";

            Logger instance = Logger.Instance;
            if (File.Exists(path))
            {
                //delete file
                if (clickedGrid.IsMouseOver == true)
                {
                    ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "download-button.png", UriKind.Relative));
                    clickedGrid.ToolTip = "Download";
                    clickedGrid.Background = new ImageBrush(bgimg);
                    File.Delete(path);
                }
            }
            else
            {
                string hash = searchResults[i].GetName();
                PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
                DownloadBody downloadBody = new(DownloadBody.Type.SongFile, hash);

                Packet packet = new(packetHeader, downloadBody);

                TxBuffer = packet.Serialize();

                App.client.Send(TxBuffer, TxBuffer.Length, App.iPEndPoint);

                instance.Log(packet, true);
                ReceiveDownloadData(hash, DownloadBody.Type.SongFile, false);

                ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "delete.png", UriKind.Relative));
                clickedGrid.ToolTip = "Delete";
                clickedGrid.Background = new ImageBrush(bgimg);
            }
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float vol = (float)Volume.Value / 100;
            outputDevice.Volume = vol;

            Volume.Value = (double)outputDevice.Volume * 100;
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
                PerformSearch();
            }
        }

        private void searchtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchtb.Text == string.Empty)
            {
                clearSearch();
                noResultstb.Visibility = Visibility.Hidden;
            }
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = searchtb;
            if (textBox != null)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                textBox.MoveFocus(tRequest);
                clearSearch();
            }
        }

        private void ToggleRepeat(object sender, RoutedEventArgs e)
        {
            if (repeatActive)
            {
                repeatActive = false;
                ImageSource repeatimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "repeat.png", UriKind.Relative));
                repeatButton.Background = new ImageBrush(repeatimg);
                rptCircle.Visibility = Visibility.Hidden;
            }
            else
            {
                ImageSource repeatimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "repeat-blue.png", UriKind.Relative));
                repeatButton.Background = new ImageBrush(repeatimg);
                rptCircle.Visibility = Visibility.Visible;
                repeatActive = true;
            }
        }

        private void rewindButton_Click(object sender, RoutedEventArgs e)
        {
            Skip(TimeSpan.FromSeconds(-ClientConstants.SkipSeconds));
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            Skip(TimeSpan.FromSeconds(ClientConstants.SkipSeconds));
        }
    }
}
