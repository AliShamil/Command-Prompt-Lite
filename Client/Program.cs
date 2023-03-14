using CommandLibrary;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
//string ipAddress = "127.0.0.1";
//int port = 12345;
//TcpClient client = new TcpClient(ipAddress, port);
//NetworkStream stream = client.GetStream();
//while (true)
//{
//    Console.Write("Enter:");
//    var text = Console.ReadLine();

//    Command myObject = new Command();
//    myObject.Text = text;
//    myObject.Param = text;
//    BinaryFormatter formatter = new BinaryFormatter();
//    byte[] buffer = new byte[1024];
//    using (var ms = new MemoryStream())
//    {
//        formatter.Serialize(ms, myObject);
//        buffer = ms.ToArray();
//    }
//    stream.Write(buffer, 0, buffer.Length);
//    stream.Flush();

//    if (text == "1")
//    {
//        byte[] receiveBuffer = new byte[1024];
//        int receivedBytes = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
//        string receivedText = Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);
//        Console.WriteLine("Received: " + receivedText);
//    }
//}


string ipAddress = "127.0.0.1";
int port = 12345;
TcpClient client = new TcpClient(ipAddress, port);
NetworkStream stream = client.GetStream();
while (true)
{
    Console.Write($"{Environment.CurrentDirectory}");
    var text = Console.ReadLine();
    
    //var t = text.Split(' ');
    
    Command myObject = new Command();
    myObject.Text = text.Trim();
    myObject.Param = text.Trim();

    string serializedObject = JsonConvert.SerializeObject(myObject);
    byte[] buffer = Encoding.UTF8.GetBytes(serializedObject);

    stream.Write(buffer, 0, buffer.Length);
    stream.Flush();

    if (text == "1")
    {
        byte[] receiveBuffer = new byte[1024];
        int receivedBytes = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
        string receivedText = Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);
        Console.WriteLine("Received: " + receivedText);
    }
}