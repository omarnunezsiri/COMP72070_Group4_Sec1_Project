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
        public static TcpClient client;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.SetFileName(Constants.TextDirectory + Constants.ClientLogsFile);
            Directory.SetCurrentDirectory("../../../");

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new(iPAddress, ClientConstants.PortNumber);

            client = new();

            await client.ConnectAsync(iPEndPoint);
        }
    }
}
