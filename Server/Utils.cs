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
    public static class Utils
    {
        public static bool CompareBitmaps(Bitmap a, Bitmap b)
        {
            if (!a.Size.Equals(b.Size)) return false;

            for (int i = 0; i < a.Width; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    if (a.GetPixel(i, j) != b.GetPixel(i, j)) return false;
                }
            }

            return true;
        }

        public static byte[] GetBitmapBytes(Bitmap bmp)
        {
            using(MemoryStream memoryStream = new())
            {
                bmp.Save(memoryStream, bmp.RawFormat);

                return memoryStream.ToArray();
            }
        }

        public static Bitmap GetBitmapFromBytes(byte[] bytes)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);

                return new Bitmap(stream);
            }
        }
    }
}
