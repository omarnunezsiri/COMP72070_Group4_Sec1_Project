using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

namespace Server
{
    public static class Utils
    {
        public static bool CompareBitmaps(Bitmap a, Bitmap b)
        {
            if (!a.Size.Equals(b.Size)) return false;

            for (int i = 0; i < a.Width; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    if (a.GetPixel(i, j) != b.GetPixel(i, j)) return false;
                }
            }

            return true;
        }

        public static byte[] GetBitmapBytes(Bitmap bmp)
        {
            using(MemoryStream memoryStream = new())
            {
                bmp.Save(memoryStream, bmp.RawFormat);

                return memoryStream.ToArray();
            }
        }

        public static Bitmap GetBitmapFromBytes(byte[] bytes)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);

                return new Bitmap(stream);
            }
        }


        /// <summary>
        /// Searches and gathers all search results of songs matching a given search term
        /// </summary>
        /// <param name="sc">SongController containing all songs</param>
        /// <param name="searchTerm">Term to search for</param>
        /// <returns>List of songs that match search criteria</returns>
        public static List<Song> SearchSong(SongController sc, String searchTerm)
        {
            List<Song> results = new List<Song>();
            Dictionary<String, Song> songs = sc.ViewSongs();

            foreach (var song in songs) 
            {
                if(song.Key.Contains(searchTerm))
                {
                    results.Add(song.Value);
                }
            }

            return results;
        }

        /// <summary>
        /// Packs a song object with its cover in a collection of bytes.
        /// </summary>
        /// <param name="song">Song to serialize</param>
        /// <param name="album">Album to get cover from</param>
        /// <returns>A collection of bytes with the packed song and cover</returns>
        private static List<Byte> GetPackedSong(Song song, Album album)
        {
            int offset = 0;

            Bitmap albumCover = album.GetImage();

            byte[] songBytes = song.Serialize();
            byte[] albumCoverBytes = GetBitmapBytes(albumCover);
            byte[] coverLengthBytes = BitConverter.GetBytes(albumCoverBytes.Length);

            byte[] joined = new byte[albumCoverBytes.Length + songBytes.Length + coverLengthBytes.Length];
            songBytes.CopyTo(joined, offset);

            offset += songBytes.Length;

            coverLengthBytes.CopyTo(joined, offset);
            offset += sizeof(int);

            albumCoverBytes.CopyTo(joined, offset);

            return joined.ToList();
        }
        
        //public static Bitmap getSongImage(Song song)            //this needs to get done. Can I do it without passing in the artist controller?
        //{

        //}

        /// <summary>
        /// Takes a given search term and outputs a completed search packet to be sent by the client.
        /// Just a higher level of abstraction for packet creation.
        /// </summary>
        /// <param name="searchTerm">The string to search for</param>
        /// <returns>Completed packet</returns>
        public static Packet GenerateClientSearchPacket(String searchTerm)
        {
            SearchBody body = new SearchBody(0, searchTerm);
            PacketHeader head = new PacketHeader(PacketHeader.SongAction.List);

            Packet pk = new Packet(head, body);

            return pk;
        }

        /// <summary>
        /// Abstraction of server response to a search request from client.
        /// Will handle gathering all the needed data for the search results and packaging it up.
        /// </summary>
        /// <param name="searchResults">List of songs found in the search</param>
        /// <param name="albumController">Album controller to get covers from</param>
        /// <returns></returns>
        public static byte[] GenerateServerSearchResponse(List<Song> searchResults, AlbumController albumController)
        {
            List<Byte> buffer = new List<Byte>();

            foreach (Song song in searchResults)
            {
                Album album = albumController.FindAlbum(song.GetAlbum());

                List<byte> packedSong = GetPackedSong(song, album);
                buffer.AddRange(packedSong);
            }

            return buffer.ToArray();
        }

        public static void unpackServerSearchResponse(Packet pk)
        {
            SearchBody body = (SearchBody)pk.body;
        }

        public static void PopulateSearchResults(byte[] rawData, List<Song> tempSongs, string imageDir)
        {
            int length = rawData.Length;
            int offset = 0;

            while(offset < length)
            {
                short songLength = BitConverter.ToInt16(rawData, offset);
                offset += sizeof(short);

                songLength -= 2; // doesn't take into account the Int16 from the length

                byte[] songBytes = new byte[songLength];
                Array.Copy(rawData, offset, songBytes, 0, songLength);
                Song tempSong = new Song(songBytes);

                offset += songLength;

                int bitmapLength = BitConverter.ToInt32(rawData, offset); 
                offset += sizeof(int);

                byte[] bitmapBytes = new byte[bitmapLength];
                Array.Copy(rawData, offset, bitmapBytes, 0, bitmapLength);
                offset += bitmapLength;

                Bitmap tempBmp = GetBitmapFromBytes(bitmapBytes);
                Bitmap bmp2 = new(tempBmp);

                tempSongs.Add(tempSong);
                FileHandler.writeImageBytes($"{imageDir}{tempSong.GetName()}.jpg", bmp2);
                tempBmp.Dispose();
            }

        }
    }
}
