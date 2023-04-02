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
    public static class FileHandler
    {
        public static bool writeMp3Bytes(string fileName, byte[] mp3data)
        {
            if (mp3data is null || mp3data.Length == 0) return false;

            using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                fs.Write(mp3data, 0, mp3data.Length);

                return true;
            }
        }

        public static byte[] readMp3Bytes(string fileName)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                MemoryStream ms = new();

                fs.CopyTo(ms);

                byte[] bytes = ms.ToArray();

                return bytes;
            }
        }

        public static bool writeImageBytes(string fileName, Bitmap bitmap)
        {
            bitmap.Save(fileName);

            return true;
        }

        public static Bitmap readImageBytes(string fileName)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            return (Bitmap)Image.FromFile(fileName);
        }

        public static void ReadAccounts(AccountController accountController, string fileName)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using (StreamReader sr = new StreamReader(fileName))
            {
                string? line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (!String.IsNullOrEmpty(line))
                    {
                        /* Parsed line: username,password */
                        string[] strs = line.Split(",");
                        accountController.AddAccount(strs[0], strs[1]);
                    }
                }
            }
        }

        public static void WriteAccounts(AccountController accountController, string fileName)
        {
            Dictionary<String, Account> collection = accountController.ViewAccounts();

            using (StreamWriter sw = new(fileName))
            {
                foreach (KeyValuePair<String, Account> kvp in collection)
                {
                    sw.WriteLine($"{kvp.Value.getUsername()},{kvp.Value.getPassword()}");
                }
            }
        }

        public static void ReadSongs(SongController songController, string fileName)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using (StreamReader sr = new StreamReader(fileName))
            {
                string? line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (!String.IsNullOrEmpty(line))
                    {
                        /* Parsed line: username,password */
                        string[] strs = line.Split(",");
                        songController.AddSong(strs[0], strs[1], strs[2], float.Parse(strs[3]));
                    }
                }
            }
        }

        public static void WriteSongs(SongController songController, string fileName)
        {
            Dictionary<String, Song> collection = songController.ViewSongs();

            using (StreamWriter sw = new(fileName))
            {
                foreach (KeyValuePair<String, Song> kvp in collection)
                {
                    sw.WriteLine($"{kvp.Value.GetName()},{kvp.Value.GetAlbum()},{kvp.Value.GetArtist()},{kvp.Value.GetDuration()}");
                }
            }
        }

        public static void ReadArtists(ArtistController artistController, string fileName, string imageDir)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using (StreamReader sr = new StreamReader(fileName))
            {
                string? line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (!String.IsNullOrEmpty(line))
                    {
                        /* Parsed line: username,password */
                        string[] strs = line.Split(",");
                        artistController.AddArtist(strs[1], (Bitmap)Image.FromFile($"{imageDir}{strs[1]}.{strs[0].ToLower()}"));
                    }
                }
            }
        }

        public static void WriteArtists(ArtistController artistController, string fileName)
        {
            Dictionary<String, Artist> collection = artistController.ViewArtists();

            using (StreamWriter sw = new(fileName))
            {
                foreach (KeyValuePair<String, Artist> kvp in collection)
                {
                    sw.WriteLine($"{kvp.Value.GetImage().RawFormat},{kvp.Value.GetName()}");
                }
            }
        }

        public static void ReadAlbums(AlbumController albumController, string fileName, string imageDir)
        {
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using (StreamReader sr = new StreamReader(fileName))
            {
                string? line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    if (!String.IsNullOrEmpty(line))
                    {
                        /* Parsed line: username,password */
                        string[] strs = line.Split(",");
                        albumController.AddAlbum(strs[1], strs[2], (Bitmap)Image.FromFile($"{imageDir}{strs[1]}.{strs[0].ToLower()}"));
                    }
                }
            }
        }

        public static void WriteAlbums(AlbumController albumController, string fileName)
        {
            Dictionary<String, Album> collection = albumController.ViewAlbums();

            using (StreamWriter sw = new(fileName))
            {
                foreach (KeyValuePair<String, Album> kvp in collection)
                {
                    sw.WriteLine($"{kvp.Value.GetImage().RawFormat},{kvp.Value.GetName()},{kvp.Value.GetArtist()}");
                }
            }
        }
    }
}
