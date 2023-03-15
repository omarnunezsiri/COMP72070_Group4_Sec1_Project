using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Song
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

        public string GetName() { return _name; }
        public string GetArtist() { return _artist; }
        public float GetDuration() { return _duration; }
        public string GetAlbum() { return _album; }

        public void SetName(string name) { _name = name; }
        public void SetArtist(string artist) { _artist = artist; }
        public void SetDuration(float duration) { _duration = duration; }
        public void SetAlbum(string album) { _album = album; }
    }
}