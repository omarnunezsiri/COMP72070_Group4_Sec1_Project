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
    public class Album : ISerializable
    {
        private string _name;
        private string _artist;
        private Bitmap _image;

        public Album()
        {
            _name = string.Empty;
            _artist = string.Empty;
            _image = (Bitmap)Image.FromFile(Constants.DefaultImageFile);
        }

        public Album(string name, string artist, Bitmap image)
        {
            _name = name;
            _artist = artist;
            _image = image;
        }

        public Album(byte[] albumBytes)
        {
            int offset = 0;
            byte len = albumBytes[offset++];

            _name = Encoding.ASCII.GetString(albumBytes, offset, len);
            offset += len;

            len = albumBytes[offset++];
            _artist = Encoding.ASCII.GetString(albumBytes, offset, len);
            offset += len;

            byte[] imageBytes = albumBytes.Skip(offset)
                                           .ToArray();

            _image = Utils.GetBitmapFromBytes(imageBytes);
        }

        public string GetName() { return _name; }
        public string GetArtist() { return _artist; }
        public Bitmap GetImage() { return _image; }

        public void SetName(string name) { _name = name; }
        public void SetArtist(string artist) { _artist = artist; }
        public void SetImage(Bitmap image) { _image = image; }

        public byte[] Serialize()
        {
            byte[] imageBytes = Utils.GetBitmapBytes(_image);

            int offset = 0;
            byte[] serialized = new byte[Constants.AlbumIndividualBytes * sizeof(byte) + _name.Length + _artist.Length + imageBytes.Length];

            byte len = Convert.ToByte(_name.Length);
            serialized[offset++] = len;

            Encoding.ASCII.GetBytes(_name).CopyTo(serialized, offset);
            offset += len;

            len = Convert.ToByte(_artist.Length);
            serialized[offset++] = len;

            Encoding.ASCII.GetBytes(_artist).CopyTo(serialized, offset);
            offset += len;

            imageBytes.CopyTo(serialized, offset);
            return serialized;
        }
    }
}
