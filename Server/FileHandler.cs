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

            using(FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                fs.Write(mp3data, 0, mp3data.Length);

                return true;
            }
        }

        public static byte[] readMp3Bytes(string fileName) 
        { 
            if (!File.Exists(fileName)) { throw new FileNotFoundException(); }

            using(FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            if(!File.Exists(fileName)) { throw new FileNotFoundException(); }

            return (Bitmap)Image.FromFile(fileName);
        }
    }
}
