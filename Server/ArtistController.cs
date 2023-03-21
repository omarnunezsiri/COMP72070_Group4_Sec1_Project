using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public ArtistController() { }

        public bool AddArtist(string artistName, Bitmap artistImage)
        {
            throw new NotImplementedException();
        }

        public Artist FindArtist(string hash) { throw new NotImplementedException(); }

        public Dictionary<string, Artist> ViewArtists() { throw new NotImplementedException(); }

        public bool DeleteArtist(string hash) { throw new NotImplementedException(); }

        public bool UpdateArtist(string hash, string criteria, object o) { throw new NotImplementedException(); }
    }
}