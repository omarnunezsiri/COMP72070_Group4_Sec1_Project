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
        public static List<Song> searchSong(SongController sc, String searchTerm)
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

        //public static Bitmap getSongImage(Song song)            //this needs to get done. Can I do it without passing in the artist controller?
        //{

        //}

        /// <summary>
        /// Takes a given search term and outputs a completed search packet to be sent by the client.
        /// Just a higher level of abstraction for packet creation.
        /// </summary>
        /// <param name="searchTerm">The string to search for</param>
        /// <returns>Completed packet</returns>
        public static Packet generateClientSearchPacket(String searchTerm)
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
        /// <returns></returns>
        public static Packet generateServerSearchResponse(List<Song> searchResults)
        {
            PacketHeader head = new PacketHeader(PacketHeader.SongAction.List);
            List<Byte> buffer = new List<Byte>();



            PacketBody body = new SearchBody(0, "null", buffer.ToArray());

            Packet pk = new Packet(head, body);
        }

        public static void unpackServerSearchResponse(Packet pk)
        {
            SearchBody body = (SearchBody)pk.body;
        }
    }
}
