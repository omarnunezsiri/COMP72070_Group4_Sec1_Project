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
    public class Artist
    {
        private string _name;
        private Bitmap _image;

        public Artist()
        {
            _name = string.Empty;
            _image = (Bitmap)Image.FromFile("default.png");
        }

        public Artist(string name, Bitmap image)
        {
            _name = name;
            _image = image;
        }

        public string GetName() { return _name; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetImage(Bitmap image) { _image = image; }
    }
}
