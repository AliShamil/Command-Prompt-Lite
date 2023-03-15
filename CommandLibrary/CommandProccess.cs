using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLibrary;

public static class CommandProccess
{
    static public string GetHelpText()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("\nproclist".PadRight(40));
        builder.Append("Show all processes");
        builder.Append("\nkill <process name>".PadRight(40));
        builder.Append("End the given process");
        builder.Append("\nrun <process name>".PadRight(40));
        builder.Append("Run the given process");
        return builder.ToString();
    }
    static public List<string> GetProcList()
    {
        var list = Process.GetProcesses()
                       .Select(p => p.ProcessName)
                       .ToList();
        return list;
    }

    static public bool TryKill(string? parameter)
    {
        if (parameter == null)
            return false;

        var canKill = false;
        var processes = Process.GetProcessesByName(parameter);

        if (processes.Length > 0)
        {
            try
            {
                foreach (var p in processes)
                    p.Kill();

                canKill = true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }
        return canKill;
    }

    static public bool TryRun(string? parameter)
    {
        if (parameter == null)
            return false;

        var canRun = false;

        try
        {
            Process.Start(parameter);
            canRun = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return canRun;

    }
    static public List<string> ToFilterInput(string input)
    {
        var temp = input.Trim().Split(' ');

        var filteredCommand = new List<string>();
        foreach (var property in temp)
        {
            if (!string.IsNullOrWhiteSpace(property))
                filteredCommand.Add(property.Trim());
        }
        return filteredCommand;
    }
}
