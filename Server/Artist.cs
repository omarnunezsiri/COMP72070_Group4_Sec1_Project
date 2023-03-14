using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Artist
    {
        private string _name;
        private Bitmap _image;

        public string GetName() { return _name; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetImage(Bitmap image) { _image = image; }
    }
}
