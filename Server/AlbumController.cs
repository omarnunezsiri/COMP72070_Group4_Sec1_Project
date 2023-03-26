using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AlbumController
    {
        Dictionary<string, Album> albums;

        public AlbumController()
        {
            albums = new();
        }

        public bool AddAlbum(string albumName, string artistName, Bitmap image)
        {
            return albums.TryAdd(albumName, new Album(albumName, artistName, image));
        }

        public Album FindAlbum(string hash)
        {
            bool found = albums.ContainsKey(hash);

            if (!found) { throw new KeyNotFoundException("Album not found."); }

            return albums[hash];
        }

        public Dictionary<string, Album> ViewAlbums() { return albums; }

        public bool DeleteAlbum(string hash) { return albums.Remove(hash); }

        public bool UpdateAlbum(string hash, string criteria, object o)
        {
            bool actionDone = false;

            Album album = FindAlbum(hash); // can throw a KeyNotFoundException exception

            if (criteria == "name")
            {
                album.SetName((string)o);
                DeleteAlbum(hash);

                if (AddAlbum(album.GetName(), album.GetArtist(), album.GetImage())) // no duplicates
                    actionDone = true;
            }
            else if(criteria == "artist")
            {
                album.SetArtist((string)o);
                actionDone = true;
            }
            else if (criteria == "image")
            {
                album.SetImage((Bitmap)o);
                actionDone = true;
            }

            return actionDone;
        }
    }
}
