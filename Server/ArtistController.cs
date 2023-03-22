using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

namespace Server
{
    public class ArtistController
    {
        Dictionary<string, Artist> artists;

        public ArtistController() 
        {
            artists = new();
        }

        public bool AddArtist(string artistName, Bitmap artistImage)
        {
            return artists.TryAdd(artistName, new Artist(artistName, artistImage));
        }

        public Artist FindArtist(string hash) 
        {
            bool found = artists.ContainsKey(hash);

            if (!found) { throw new KeyNotFoundException("Artist not found."); }

            return artists[hash];
        }

        public Dictionary<string, Artist> ViewArtists() { return artists; }

        public bool DeleteArtist(string hash) { return artists.Remove(hash); }

        public bool UpdateArtist(string hash, string criteria, object o) 
        {
            bool actionDone = false;

            Artist artist = FindArtist(hash); // can throw a KeyNotFoundException exception

            if (criteria == "name")
            {
                artist.SetName((string)o);
                DeleteArtist(hash);

                if (AddArtist(artist.GetName(), artist.GetImage())) // no duplicates
                    actionDone = true;
            }
            else if (criteria == "image")
            {
                artist.SetImage((Bitmap)o);
                actionDone = true;
            }   

            return actionDone;
        }
    }
}