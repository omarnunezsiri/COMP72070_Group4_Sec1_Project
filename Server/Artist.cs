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
    public class Artist : ISerializable
    {
        private string _name;
        private Bitmap _image;

        public Artist()
        {
            _name = string.Empty;
            _image = (Bitmap)Image.FromFile(Constants.DefaultImageFile);
        }

        public Artist(string name, Bitmap image)
        {
            _name = name;
            _image = image;
        }

        public Artist(byte[] artistBytes)
        {
            int offset = 0;
            byte len = artistBytes[offset++];

            _name = Encoding.ASCII.GetString(artistBytes, offset, len);
            offset += len;

            byte[] imageBytes = artistBytes.Skip(offset)
                                           .ToArray();

            _image = Utils.GetBitmapFromBytes(imageBytes);
        }

        public string GetName() { return _name; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetImage(Bitmap image) { _image = image; }

        public byte[] Serialize()
        {
            byte[] imageBytes = Utils.GetBitmapBytes(_image);

            int offset = 0;
            byte[] serialized = new byte[sizeof(byte) + _name.Length + imageBytes.Length];

            byte len = Convert.ToByte(_name.Length);
            serialized[offset++] = len;

            byte[] nameBytes = Encoding.ASCII.GetBytes(_name);
            nameBytes.CopyTo(serialized, offset);
            offset += len;

            imageBytes.CopyTo(serialized, offset);

            return serialized;
        }
    }
}
