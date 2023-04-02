using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{ 
    public class Song : ISerializable
    {
        private string _name;
        private string _artist;
        private float _duration;
        private string _album;

        public Song()
        {
            _name = string.Empty;
            _artist = string.Empty;
            _duration = 0.0f;
            _album = string.Empty;
        }

        public Song(string name, string album, string artist, float duration)
        {
            _name = name;
            _artist = artist;
            _duration = duration;
            _album = album;
        }

        public Song(byte[] songBytes)
        {
            int offset = 0;
            byte len = songBytes[offset++];

            _name = Encoding.ASCII.GetString(songBytes, offset, len);
            offset += len;

            len = songBytes[offset++];
            _artist = Encoding.ASCII.GetString(songBytes, offset, len);

            offset += len;

            _duration = BitConverter.ToSingle(songBytes, offset);
            offset += sizeof(float);

            len = songBytes[offset++];
            _album = Encoding.ASCII.GetString(songBytes, offset, len);
        }

        public string GetName() { return _name; }
        public string GetArtist() { return _artist; }
        public float GetDuration() { return _duration; }
        public string GetAlbum() { return _album; }

        public void SetName(string name) { _name = name; }
        public void SetArtist(string artist) { _artist = artist; }
        public void SetDuration(float duration) { _duration = duration; }
        public void SetAlbum(string album) { _album = album; }

        public byte[] Serialize()
        {
            int offset = 0;

            // [TOTALBYTES] [NAMELENGTH 1byte] [NAME nBytes] | [ARTISTLENGTH 1byte] [ARTIST nBytes] | [DURATION 4bytes] | [ALBUMLENGTH 1byte] [ALBUM nBytes]
            byte[] songBytes = new byte[sizeof(Int16) + Constants.SongIndividualBytes * sizeof(byte) + _name.Length + _artist.Length + _album.Length + sizeof(float)];

            Int16 totalBytes = Convert.ToInt16(songBytes.Length);

            byte[] bytesFromTotal = BitConverter.GetBytes(totalBytes);
            bytesFromTotal.CopyTo(songBytes, offset);

            offset += sizeof(Int16);

            byte len = Convert.ToByte(_name.Length);
            songBytes[offset++] = len;

            byte[] nameBytes = Encoding.ASCII.GetBytes(_name);
            nameBytes.CopyTo(songBytes, offset);
            offset += len;

            len = Convert.ToByte(_artist.Length);
            songBytes[offset++] = len;

            byte[] artistBytes = Encoding.ASCII.GetBytes(_artist);
            artistBytes.CopyTo(songBytes, offset);
            offset += len;

            byte[] durationBytes = BitConverter.GetBytes(_duration);
            durationBytes.CopyTo(songBytes, offset);
            offset += durationBytes.Length;

            len = Convert.ToByte(_album.Length);
            songBytes[offset++] = len;

            byte[] albumBytes = Encoding.ASCII.GetBytes(_album);
            albumBytes.CopyTo(songBytes, offset);

            return songBytes;
        }
    }
}