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