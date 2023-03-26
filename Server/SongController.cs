using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SongController
    {
        Dictionary<string, Song> songs;

        public SongController()
        {
            songs = new();
        }

        public bool AddSong(string name, string album, string artist, float duration)
        {
            return songs.TryAdd(name, new Song(name, album, artist, duration));
        }

        public Song FindSong(string hash)
        {
            bool found = songs.ContainsKey(hash);

            if (!found) { throw new KeyNotFoundException("Song not found."); }

            return songs[hash];
        }

        public Dictionary<string, Song> ViewSongs() { return songs; }

        public bool DeleteSong(string hash) { return songs.Remove(hash); }

        public bool UpdateSong(string hash, string criteria, object o)
        {
            bool actionDone = false;

            Song song = FindSong(hash); // can throw a KeyNotFoundException exception

            if (criteria == "name")
            {
                song.SetName((string)o);
                DeleteSong(hash);

                if (AddSong(song.GetName(), song.GetAlbum(), song.GetArtist(), song.GetDuration())) // no duplicates
                    actionDone = true;
            }
            else if (criteria == "album")
            {
                song.SetAlbum((string)o);
                actionDone = true;
            }
            else if(criteria == "artist")
            {
                song.SetArtist((string)o);
                actionDone = true;
            }
            else if(criteria == "duration")
            {
                song.SetDuration((float)o);
                actionDone = true;
            }

            return actionDone;
        }
    }
}
