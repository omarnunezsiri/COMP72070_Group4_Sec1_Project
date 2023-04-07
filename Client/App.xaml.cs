using Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UdpClient client;
        public static IPEndPoint iPEndPoint;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.SetFileName(Constants.TextDirectory + Constants.ClientLogsFile);

            /* Defaults to the Client project directory instead of the Debug (which is ignored by our .gitignore file). */
            Directory.SetCurrentDirectory("../../../");

            /* Clears temp files before load */
            DirectoryInfo directoryInfo = new DirectoryInfo(Constants.TempDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            iPEndPoint = new(iPAddress, ClientConstants.PortNumber);

            client = new();
        }
    }
}
