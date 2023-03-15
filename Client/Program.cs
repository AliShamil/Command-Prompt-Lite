using CommandLibrary;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using System.Net;

var port = 12345;
TcpClient client = null;
var count = 0;
tryconnect:
try
{
    client = new TcpClient("127.0.0.1", port);
    Console.Clear();
    Console.WriteLine(@"Connected successfuly!
Press any key to continue...");
    Console.ReadKey();
    Console.Clear();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

if (client == null)
{
    if (count != 6)
    {
        ++count;
        Console.Clear();
        Console.WriteLine($"Trying to connect {count}/6");
        goto tryconnect;
    }
    return;
}

var stream = client.GetStream();

var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);

var input = string.Empty;

while (true)
{
    input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine(@"Please enter command.
Press any key to continue...");
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
        "exit" => CommandText.Exit,
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
                    Console.WriteLine(@"Parameter is not acceptable for 'proclist' command.
Press any key to continue...");
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
                    Console.WriteLine(@"Proclist cannot be loaded!
Press any key to continue...");
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
                    Console.WriteLine(@"Parameter is not acceptable for 'help' command.
Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                var command = JsonConvert.SerializeObject(new Command { Text = commandText ,Param = param});

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
                    Console.WriteLine(@"<process name> must be given for using 'kill' command.
Press any key to continue...");
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
                    Console.WriteLine(@$"'{param}' process succesfully ended!
Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine($@"'{param}' process cannot be ended!
Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                break;
            }
        case CommandText.Run:
            {
                if (string.IsNullOrWhiteSpace(param))
                {
                    Console.WriteLine(@"<process name> must be given for using 'run' command.
Press any key to continue...");
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
                    Console.WriteLine($@"'{param}' process succesfully started!
Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine(@"Process cannot be started!
Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                break;
            }
        case CommandText.Unkown:
            {
                Console.WriteLine(@"Please use 'help' command.
Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
                break;
            }
       case CommandText.Exit:
            {
                Console.WriteLine("Closing...");
                await Task.Delay(1000);
                Console.Clear() ;
                client.Close();
                Environment.Exit(0);
                break;
            }

    }

}