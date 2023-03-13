using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CommandLibrary;

int port = 12345;
TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
listener.Start();

TcpClient remoteClient = listener.AcceptTcpClient();
NetworkStream remoteStream = remoteClient.GetStream();
while (true)
{
    BinaryFormatter formatter = new BinaryFormatter();
    byte[] receivedBuffer = new byte[1024];
    int receivedBytes = remoteStream.Read(receivedBuffer, 0, receivedBuffer.Length);


    using (var ms = new MemoryStream(receivedBuffer))
    {
        Command receivedObject = (Command)formatter.Deserialize(ms);
        Console.WriteLine("Received command: Text={0}, Param={1}", receivedObject.Text, receivedObject.Param);

        if (receivedObject.Text == "1")
        {
            byte[] sendBuffer = Encoding.ASCII.GetBytes("Salam Aleykum");
            remoteStream.Write(sendBuffer, 0, sendBuffer.Length);
            remoteStream.Flush();
        }
    }
}