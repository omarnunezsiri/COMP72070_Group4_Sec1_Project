using NAudio.Wave;

namespace ClientLibrary
{
    public class AudioHandler
    {
        public void playSong(String songFileName)
        {
            var song = new AudioFileReader(songFileName);
            var outputDevice = new WaveOutEvent();

            outputDevice.Init(song);
            outputDevice.Play();

            while (outputDevice.PlaybackState == PlaybackState.Playing) //this is dumb lol
            {
                Thread.Sleep(1000);
            }
        }

        public bool cleanupTempFile()
        {
            bool success = false;

        }
    }
}