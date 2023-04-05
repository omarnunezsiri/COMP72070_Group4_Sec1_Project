using NAudio.Utils;
using NAudio.Wave;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

        State playingState = new();
        WaveOutEvent outputDevice = new();

        string currentSongLocation;

        CancellationTokenSource cancellationToken;
        Task playTask;

        /* Data Commucations */
        byte[] TxBuffer;
        byte[] RxBuffer;
        NetworkStream stream;
        MediaControlBody.State state;

        //search stuff
        //this list "searchResults" should be populated with the search results from the server
        public List<Song> searchResults { get; set; } = new List<Song> { };

        List<Grid> buttonList = new List<Grid>();   //list for storing search result buttons

        //next up is a bunch of trash that defines the size and where the buttons appear


        int currentY = 0; //Y location of the current button
        int Xloc = 0; //X location to put every button

        //button dimensions
        const int WIDTH = 400;
        const int HEIGHT = 40;

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

            stream = App.client.GetStream();
            RxBuffer = new byte[Constants.Mp3BufferMax + Constants.CoverBufferMax];
            state = MediaControlBody.State.Idle;

            currentSongLocation = "";
            cancellationToken = new CancellationTokenSource();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out of your account?", "Sign Out", MessageBoxButton.YesNo);
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
                mediaControlBody = new(MediaControlBody.Action.Pause);
                StateTransition(packetHeader, mediaControlBody);
                playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
            }
            else //SONG IS PLAYING
            {
                if(!string.IsNullOrEmpty(currentSongLocation))
                {
                    mediaControlBody = new(MediaControlBody.Action.Play);
                    StateTransition(packetHeader, mediaControlBody);
                    playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "pause-button.png", UriKind.Relative));

                    if (playTask is null)
                        playTask = PlayWorkerAsync(currentSongLocation, cancellationToken.Token);
                    else if(playTask.IsCanceled || playTask.IsCompleted)
                    {
                        cancellationToken = new CancellationTokenSource();
                        playTask = PlayWorkerAsync(currentSongLocation, cancellationToken.Token);
                    }
                }
            }
        }

        private async Task PlayWorkerAsync(string songLocation, CancellationToken cancellationToken)
        {
            using (var audioFile = new AudioFileReader(songLocation))
            {
                TimeSpan totalDuration = audioFile.TotalTime;

                await Dispatcher.InvokeAsync(() =>
                {
                    progressBar.Maximum = audioFile.Length;

                    endTime.Content = string.Format("{0:D2}:{1:D2}",
                        totalDuration.Minutes,
                        totalDuration.Seconds);

                    outputDevice.Volume = (float)Volume.Value / 100;
                });

                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing && !cancellationToken.IsCancellationRequested)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (outputDevice.PlaybackState is PlaybackState.Playing)
                        {
                            progressBar.Value = playingState.Position = outputDevice.GetPosition();
                            TimeSpan currentPosition = outputDevice.GetPositionTimeSpan();
                            startTime.Content = string.Format("{0:D2}:{1:D2}",
                                                currentPosition.Minutes,
                                                currentPosition.Seconds);
                            Volume.Value = (double)outputDevice.Volume * 100;
                        }
                    });

                    if (state == MediaControlBody.State.Paused)
                    {
                        if (outputDevice.PlaybackState != PlaybackState.Stopped)
                        {
                            outputDevice.Pause();
                            while (state == MediaControlBody.State.Paused && !cancellationToken.IsCancellationRequested)
                            {
                                await Task.Delay(100);
                            }

                            outputDevice.Play();
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));
                    progressBar.Value += (audioFile.Position - progressBar.Value);
                });
            }
        }

        private void PlayCleanup()
        {
            playImg.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "play-button.png", UriKind.Relative));

            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                outputDevice.Stop();
            }

            outputDevice.Dispose();

            state = MediaControlBody.State.Idle;
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
            Debug.WriteLine(clickedItem);
            //clearSearch();
            int i = Int32.Parse(clickedItem);

            string hash = searchResults[i].GetName();
            string album = searchResults[i].GetAlbum();
            string imagePath = Constants.TempDirectory + $"{searchResults[i].GetAlbum()}.jpg";
            string localMp3Path = Constants.Mp3sDirectory + $"{hash}.mp3";

            if (!File.Exists(localMp3Path))
            {
                PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
                DownloadBody downloadBody = new(DownloadBody.Type.SongFile, hash);

                Packet packet = new(packetHeader, downloadBody);

                TxBuffer = packet.Serialize();

                stream.Write(TxBuffer);

                currentSongLocation = ReceiveDownloadData(packet, hash, DownloadBody.Type.SongFile, true);
            }
            else
            {
                currentSongLocation = localMp3Path;
            }

            BitmapImage bitmap = LoadRuntimeBitmap(imagePath);
            coverImage.Source = bitmap;

            artist.Content = searchResults[i].GetArtist();
            songName.Content = hash;

            if (playTask is not null && !playTask.IsCompleted)
            {
                cancellationToken.Cancel();
                await playTask;
                PlayCleanup();
            }

            playButton_Click(sender, e);
        }


        private void StateTransition(PacketHeader packetHeader, MediaControlBody mediaControlBody)
        {
            Packet pausePacket = new(packetHeader, mediaControlBody);

            TxBuffer = pausePacket.Serialize();

            stream.Write(TxBuffer);

            int read = stream.Read(RxBuffer);

            byte[] receivedBuffer = new byte[read];
            Array.Copy(RxBuffer, receivedBuffer, read);

            pausePacket = new(receivedBuffer);
            mediaControlBody = (MediaControlBody)pausePacket.body;

            state = mediaControlBody.GetState();
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float vol = (float)Volume.Value / 100;
            outputDevice.Volume = vol;

            Volume.Value = (double)outputDevice.Volume * 100;
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
            string input = searchtb.Text;
            bool searchText = input.Contains(input, StringComparison.OrdinalIgnoreCase);
            if (searchText == true)
            {
                PacketHeader searchHeader = new(PacketHeader.SongAction.List);
                SearchBody searchBody = new(0x00000000, input);

                Packet searchPacket = new(searchHeader, searchBody);

                TxBuffer = searchPacket.Serialize();

                Logger instance = Logger.Instance;
                instance.Log(searchPacket, true);

                stream.Write(TxBuffer);

                int read = stream.Read(RxBuffer);

                byte[] receivedBuffer = new byte[read];
                Array.Copy(RxBuffer, receivedBuffer, read);

                searchPacket = new(receivedBuffer);
                SearchBody sb = (SearchBody)searchPacket.body;
                instance.Log(searchPacket, false);

                Utils.PopulateSearchResults(sb.GetResponse(), resultList);
                ReceiveSongCovers(resultList);
            }
        }

        private void ReceiveSongCovers(List<Song> results)
        {
            Logger instance = Logger.Instance;

            foreach (Song song in results)
            {
                string hash = song.GetAlbum();

                PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Download);
                DownloadBody db = new(DownloadBody.Type.AlbumCover, hash);

                Packet packet = new(packetHeader, db);
                TxBuffer = packet.Serialize();

                stream.Write(TxBuffer);

                ReceiveDownloadData(packet, hash, db.GetType(), true);
            }
        }

        private string ReceiveDownloadData(Packet packet, string hash, DownloadBody.Type type, bool isTemp)
        {
            /* Determines the path to open the Resource from (AlbumCover or SongFile) */
            string toOpen = "";
            if (type == DownloadBody.Type.AlbumCover)
            {
                toOpen = (isTemp ? Constants.TempDirectory : Constants.ImagesDirectory) + $"{hash}.jpg";
            }
            else if (type == DownloadBody.Type.SongFile)
            {
                toOpen = (isTemp ? Constants.TempDirectory : Constants.Mp3sDirectory) + $"{hash}.mp3";
            }


            Logger instance = Logger.Instance;
            bool reachedTotalBlocks = false;

            using (FileStream fs = File.Open(toOpen, FileMode.Create))
            {
                do
                {
                    int read = stream.Read(RxBuffer);
                    Debug.WriteLine(read);

                    byte[] receivedBuffer = new byte[read];
                    Array.Copy(RxBuffer, receivedBuffer, read);

                    packet = new(receivedBuffer);
                    instance.Log(packet, false);

                    DownloadBody receivedBody = (DownloadBody)packet.body;
                    fs.Write(receivedBody.GetData(), 0, (int)receivedBody.GetDataByteCount());

                    if (!(receivedBody.GetBlockIndex() < receivedBody.GetTotalBlocks() - 1))
                        reachedTotalBlocks = true;
                } while (!reachedTotalBlocks);
            }

            return toOpen;
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

                if (File.Exists($"./Assets/Mp3/{searchResults[i].GetName()}.mp3"))
                {
                    ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "delete.png", UriKind.Relative));
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
                    downloadButton.Background = new ImageBrush(bgimg);

                }
                


                //add all elements to grid
                newGrid.Children.Add(albumCover);             //TEMP COMMENTING OUT, NEED TO FIGURE OUT WHERE THESE IMAGES ARE GETTING PUT
                newGrid.Children.Add(songName);
                newGrid.Children.Add(artistName);
                newGrid.Children.Add(downloadButton);

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

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedGrid = (Button)sender;
            string clickedItem = clickedGrid.Name.Remove(0, 4);
            Debug.WriteLine(clickedItem);
            int i = Int32.Parse(clickedItem);
            BitmapImage bitmap = new BitmapImage();
            //clickedGrid.ImageSource = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "download-button.png", UriKind.Relative));

            String path = $"./Assets/Mp3/{searchResults[i].GetName()}.mp3";

            if (File.Exists(path))
            {
                //delete file
                if (clickedGrid.IsMouseOver == true)
                {
                    ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "download-button.png", UriKind.Relative));
                    clickedGrid.Background = new ImageBrush(bgimg);
                    //clickedGrid.Background = new SolidColorBrush(Color.FromRgb(255, 160, 122)); //red
                    //clickedGrid.Content = "X";
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

                stream.Write(TxBuffer);

                ReceiveDownloadData(packet, hash, DownloadBody.Type.SongFile, false);

                ImageSource bgimg = new BitmapImage(new Uri(ClientConstants.ImagesDirectory + "delete.png", UriKind.Relative));
                clickedGrid.Background = new ImageBrush(bgimg);

                //clickedGrid.Background = new SolidColorBrush(Color.FromRgb(152, 251, 152)); //green
                //clickedGrid.Content = "D";
            }
            //clickedGrid.Content = "X";
        }
    


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
            if (searchResults.Count == 0)
            {
                noResultstb.Visibility = Visibility.Visible;
            }
            if (searchtb.Text == string.Empty)
            {
                clearSearch();
                noResultstb.Visibility = Visibility.Hidden;
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
    }
}
