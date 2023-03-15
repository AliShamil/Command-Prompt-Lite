using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CommandLibrary;
using Newtonsoft.Json;
using System.Diagnostics;

var ip = IPAddress.Parse("127.0.0.1");
var port = 12345;
var listener = new TcpListener(ip, port);

listener.Start(10);



while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();

    Console.WriteLine($"Client {client.Client.RemoteEndPoint} accepted");

    await Task.Run(() =>
    {
        var stream = client.GetStream();
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);

        while (true)
        {
            var myObject = br.ReadString();

            var cmd = JsonConvert.DeserializeObject<Command>(myObject);

            if (cmd is null)
                return;

            switch (cmd.Text)
            {
                case CommandText.Help:
                    {                        
                        bw.Write(CommandProccess.GetHelpText());
                        stream.Flush();
                        break;
                    }
                case CommandText.Proclist:
                    {
                        var procList = JsonConvert.SerializeObject(CommandProccess.GetProcList());
                        bw.Write(procList);
                        stream.Flush();
                        break;
                    }
                case CommandText.Kill:
                    {
                        bw.Write(CommandProccess.TryKill(cmd.Param));
                        break;
                    }
                case CommandText.Run:
                    {
                        bw.Write(CommandProccess.TryRun(cmd.Param));
                        break;
                    }
                case CommandText.Unkown:
                    break;
            }
        }
    });
}