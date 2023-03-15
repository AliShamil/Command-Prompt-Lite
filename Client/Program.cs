using CommandLibrary;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using System.Net;

var port = 12345;
TcpClient client = null;
NetworkStream stream = null;
try
{
    client = new TcpClient("127.0.0.1", port);
    stream =client.GetStream();
}
catch (Exception ex)
{
    Console.WriteLine("Hello Babe");
}
if (stream == null)
    return;
var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);

var input = string.Empty;

while (true)
{
    input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Please enter command./nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
        continue;
    }

    var filteredInput = CommandProccess.ToFilterInput(input);


    CommandText commandText = filteredInput[0].ToLower() switch
    {
        "proclist" => CommandText.Proclist,
        "kill" => CommandText.Kill,
        "run" => CommandText.Run,
        "help" => CommandText.Help,
        _ => CommandText.Unkown
    };

    string? param = string.Empty;

    if (filteredInput.Count == 2)
        param = filteredInput[1];

    switch (commandText)
    {
        case CommandText.Proclist:
            {
                if (!string.IsNullOrWhiteSpace(param))
                {
                    Console.WriteLine("Parameter is not acceptable for 'proclist' command./nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                var command = JsonConvert.SerializeObject(new Command { Text = commandText, Param = param });

                bw.Write(command);

                await Task.Delay(50);

                var result = br.ReadString();

                var list = JsonConvert.DeserializeObject<List<string>>(result);

                if (list is null)
                {
                    Console.WriteLine("Proclist cannot be loaded!/nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                Console.WriteLine();

                foreach (var processName in list)
                    Console.WriteLine(processName);

                Console.ReadKey();
                Console.Clear();
                break;
            }

        case CommandText.Help:
            {
                if (!string.IsNullOrWhiteSpace(param))
                {
                    Console.WriteLine("Parameter is not acceptable for 'help' command./nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                var command = JsonConvert.SerializeObject(new Command { Text = commandText, Param = param });

                bw.Write(command);

                await Task.Delay(30);

                var result = br.ReadString();
                Console.WriteLine(result);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
                break;
            }

        case CommandText.Kill:
            {

                if (string.IsNullOrWhiteSpace(param))
                {
                    Console.WriteLine("<process name> must be given for using 'kill' command./nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                var command = JsonConvert.SerializeObject(new Command { Text = commandText, Param = param });

                bw.Write(command);

                await Task.Delay(30);

                var result = br.ReadBoolean();
                if (result is true)
                {
                    Console.WriteLine($"'{param}' process succesfully ended!/nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine($"'{param}' process cannot be ended! \nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                break;
            }
        case CommandText.Run:
            {
                if (string.IsNullOrWhiteSpace(param))
                {
                    Console.WriteLine("<process name> must be given for using 'run' command./nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                var command = JsonConvert.SerializeObject(new Command { Text = commandText, Param = param });

                bw.Write(command);

                await Task.Delay(30);

                var result = br.ReadBoolean();
                if (result is true)
                {
                    Console.WriteLine($"'{param}' process succesfully started!\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Process cannot be started!\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                break;
            }
        case CommandText.Unkown:
            {
                Console.WriteLine("Please use 'help' command.\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
                break;
            }

    }

}