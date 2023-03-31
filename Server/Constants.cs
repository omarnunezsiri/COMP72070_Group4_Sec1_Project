using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Constants
    {
        public const int SongIndividualBytes = 3;
        public const int AccountIndividualBytes = 2;
        public const int AlbumIndividualBytes = 2;
        public const int PortNumber = 27500;
        public const string DefaultImageFile = "default.png";
        public const string Mp3sDirectory = "Assets/Mp3/";
        public const string TextDirectory = "Assets/Text Files/";
        public const string ImagesDirectory = "Assets/Images/";
        public const string AccountsFile = "accounts.txt";
        public const string SongsFile = "songs.txt";
        public const string ArtistsFile = "artists.txt";
        public const string AlbumsFile = "albums.txt";
        public const string ServerLogsFile = "ServerLogs.txt";
        public const string ClientLogsFile = "ClientLogs.txt";
    }
}
