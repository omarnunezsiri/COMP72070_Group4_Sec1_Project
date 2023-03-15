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
    public class Album
    {
        private string _name;
        private string _artist;
        private Bitmap _image;

        public Album()
        {
            _name = string.Empty;
            _artist = string.Empty;
            _image = (Bitmap)Image.FromFile("default.png");
        }

        public Album(string name, string artist, Bitmap image)
        {
            _name = name;
            _artist = artist;
            _image = image;
        }

        public string GetName() { return _name; }
        public string GetArtist() { return _artist; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetArtist(string artist) { _artist = artist; }
        public void SetImage(Bitmap image) { _image = image; }
    }
}
