using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Album
    {
        private string _name;
        private string _artist;
        private Bitmap _image;

        public string GetName() { return _name; }
        public string GetArtist() { return _artist; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetArtist(string artist) { _artist = artist; }
        public void SetImage(Bitmap image) { _image = image; }
    }
}
