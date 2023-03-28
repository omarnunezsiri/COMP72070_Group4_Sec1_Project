using System.Net.Sockets;
using System.Net;
using Server;

PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
PacketBody packetBody = new Account("user", "password");
Packet packet = new(packetHeader, packetBody);

byte[] bytes = packet.Serialize();

Console.WriteLine("\npacket info when received from client: \n");
Packet anotherPacket = new(bytes);
Console.WriteLine(anotherPacket.header.GetPacketType());
Console.WriteLine(anotherPacket.header.GetAccountAction());

Account account = (Account)anotherPacket.body;
Console.WriteLine(account.getUsername());
Console.WriteLine(account.getPassword());
Console.WriteLine(account.getStatus());

account.setStatus(Account.Status.Success);

byte[] anotherBytes = anotherPacket.Serialize();
Packet clientPacket = new(anotherBytes);

Console.WriteLine("\npacket info when client receives it:\n");
Console.WriteLine(clientPacket.header.GetPacketType());
Console.WriteLine(clientPacket.header.GetAccountAction());

Account account2 = (Account)clientPacket.body;
Console.WriteLine(account2.getUsername());
Console.WriteLine(account2.getPassword());
Console.WriteLine(account2.getStatus());

IPEndPoint ipEndPoint = new(IPAddress.Any, Constants.PortNumber);
TcpListener listener = new(ipEndPoint);

try
{
    Console.WriteLine("---- Silly Music Player Running on SSP (3.5.2) ----\n");
    listener.Start();

    // keep listening for connections until interrupted
    while (true)
    {
        Console.WriteLine("Waiting for Client Connection...");
        using TcpClient handler = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Connection Established! Waiting for login/signup...\n");

        await using NetworkStream stream = handler.GetStream();

        byte[] buffer = new byte[1024];
        int receivedBytes;
        int offset;

        // Connection will run until Client disconnects, then goes back to listening
        while ((receivedBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            
            //offset = 0;
            //PacketHeader packetHeader = new(buffer[offset++]); // header located in the first byte of the packet

            //PacketHeader.Type packetType = packetHeader.GetPacketType();

            //if(packetType == PacketHeader.Type.Account)
            //{
            //    PacketHeader.AccountAction accountAction = packetHeader.GetAccountAction();

            //    if (accountAction is not 0)
            //    {
            //        switch (accountAction)
            //        {
            //            case PacketHeader.AccountAction.SignUp:

            //                break;
            //            case PacketHeader.AccountAction.LogIn:
            //                break;
            //        }
            //    }
            //}
            //else if(packetType == PacketHeader.Type.Song)
            //{

            //}

        }

        Console.WriteLine("\n Closing current Client Connection \n");
    }

}
finally
{
    listener.Stop();
}