using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Logger
    {
        private static string _fileName;
        private Logger() { _fileName = ""; }
        public static Logger Instance { get; } = new Logger();
        public static void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public void Log(Packet packet, bool toSend)
        {
            string logMessage = "";

            PacketHeader header = packet.header;
            PacketBody body = packet.body;

            logMessage += DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt -");

            PacketBody.Role role = body.role;
            PacketBody.Role oppositeRole = role == PacketBody.Role.Server ? PacketBody.Role.Client : PacketBody.Role.Server;
            string sentReceived = toSend == true ? "Sent" : "Received";
            string toFrom = toSend == true ? $"to {oppositeRole}" : $"from {role}";

            logMessage += $" {sentReceived} ";
            string reqRes = role == PacketBody.Role.Client ? "Request" : "Response";

            PacketHeader.Type type = header.GetPacketType();
            if (type == PacketHeader.Type.Account)
            {
                Account getAccount = (Account)body;
                switch (header.GetAccountAction())
                {
                    case PacketHeader.AccountAction.SignUp:
                        logMessage += $"Sign Up {reqRes} {toFrom} for ";
                        break;
                    case PacketHeader.AccountAction.LogIn:


                        if (getAccount.getPassword().Length > 0)
                        {
                            logMessage += $"Log In {reqRes} {toFrom} for ";
                        }
                        else
                        {
                            logMessage += $"Forgot Password {reqRes} {toFrom} for ";
                        }
                        break;
                    case PacketHeader.AccountAction.NotApplicable:
                        logMessage += $"Reset Password {reqRes} {toFrom} for ";
                        break;
                }

                logMessage += $"username ({getAccount.getUsername()}), password ({getAccount.getPassword()})";
                if(role == PacketBody.Role.Server)
                    logMessage += $" (Status: {getAccount.getStatus()})";
            }
            else if(type == PacketHeader.Type.Song)
            {
                //toFrom = toSend == true ? $"to {role}" : $"from {oppositeRole}";
                //role = oppositeRole;
                //reqRes = role == PacketBody.Role.Client ? "Request" : "Response";

                switch (header.GetSongAction())
                {
                    case PacketHeader.SongAction.Sync:
                        SyncBody syncBody = (SyncBody)body;
                        logMessage += $"Sync {reqRes} {toFrom}: Current play time ({syncBody.GetTimecode()}), Stream state ({syncBody.GetState()})";
                        break;

                    case PacketHeader.SongAction.Media:
                        MediaControlBody mediaControlBody = (MediaControlBody)body;

                        logMessage += $"Media {reqRes} {toFrom}: Action ({mediaControlBody.GetAction()})";

                        if (role == PacketBody.Role.Server)
                            logMessage += $" Current Media State ({mediaControlBody.GetState()})";
                        break;
                    case PacketHeader.SongAction.Download:
                        DownloadBody downloadBody = (DownloadBody)body;

                        logMessage += $"Download {reqRes} {toFrom}: Type ({downloadBody.GetType()}), Hash ({downloadBody.GetHash()})";

                        if (role == PacketBody.Role.Server)
                            logMessage += $" Block Index ({downloadBody.GetBlockIndex()}), Total Blocks ({downloadBody.GetTotalBlocks()}), Data Byte Count ({downloadBody.GetDataByteCount()})";
                        break;
                    case PacketHeader.SongAction.List:
                        SearchBody searchBody = (SearchBody)body;
                        logMessage += $"List/Search/Filter {reqRes} {toFrom}: Filter ({searchBody.GetFilter()}), Context ({searchBody.GetContext()})";

                        if (role == PacketBody.Role.Server)
                            logMessage += $" Data Byte Count ({searchBody.GetResponse().Length})";
                        break;
                }
            }

            logMessage += Environment.NewLine;
            Console.WriteLine(logMessage);
            File.AppendAllText(_fileName, logMessage);
        }
    }
}
