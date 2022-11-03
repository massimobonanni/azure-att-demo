using System;
using System.CommandLine;
using static System.Net.WebRequestMethods;


public class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample command-line app");
        var sub1Command = new Command("sub1", "First-level subcommand");
        rootCommand.Add(sub1Command);
        var sub1aCommand = new Command("sub1a", "Second level subcommand");
        sub1Command.Add(sub1aCommand);

        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("Hello world!");
        });

        sub1Command.SetHandler( () =>
        {
            Console.WriteLine("Hello sub1!");
        });

        sub1aCommand.SetHandler(() =>
        {
            Console.WriteLine("Hello sub1a!");
        });

        await rootCommand.InvokeAsync(args);
    }
}
//
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/define-commands