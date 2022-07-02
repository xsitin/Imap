using System;
using System.Collections.Generic;

namespace Imap
{
    public class CommandLineOptions
    {
        public int port { get; set; } = 143;
        public string Address { get; set; }
        public bool ssl { get; set; }
        public string user { get; set; }
        public int begin { get; set; } = 0;
        public int end { get; set; } = -1;

        public string password { get; set; }

        public static CommandLineOptions ParseArgs(string[] args)
        {
            if (args.Length < 1) PrintHelp();
            var options = new CommandLineOptions();
            var argsQueue = new Queue<string>(args);
            while (argsQueue.Count > 0)
                switch (argsQueue.Dequeue())
                {
                    case "--help":
                    case "-h":
                        PrintHelp();
                        break;
                    case "--ssl":
                        options.ssl = true;
                        break;
                    case "-s":
                    case "--server":
                        options.ParseServer(argsQueue.Dequeue());
                        break;
                    case "-n":
                        options.ParseRange(argsQueue);
                        break;
                    case "-u":
                    case "--user":
                        options.ParseUser(argsQueue.Dequeue());
                        break;
                    case "-p":
                    case "--password":
                        options.ParsePassword(argsQueue.Dequeue());
                        break;
                    default:
                        Console.WriteLine($"Incorrect argument!");
                        PrintHelp();
                        break;
                }


            return options;
        }

        private void ParseServer(string serverString)
        {
            var splinted = serverString.Split(':', StringSplitOptions.RemoveEmptyEntries);
            Address = splinted[0];
            if (splinted.Length <= 1) return;
            if (!int.TryParse(splinted[1], out var result))
            {
                Console.WriteLine("Invalid server port!");
                Environment.Exit(1);
            }

            port = result;
        }

        private void ParsePassword(string password) => this.password = password;

        private void ParseUser(string userString) => user = userString;

        private void ParseRange(Queue<string> args)
        {
            if (!int.TryParse(args.Dequeue(), out var bResult) || bResult < 0)
            {
                Console.WriteLine("Invalid range begin!");
                Environment.Exit(1);
            }

            begin = bResult;
            if (args.Count < 0 || args.Peek().StartsWith('-'))
                return;

            if (!int.TryParse(args.Dequeue(), out var eResult) || eResult < bResult)
            {
                Console.WriteLine("Invalid range end!");
                Environment.Exit(1);
            }

            end = eResult;
        }


        private static void PrintHelp()
        {
            Console.WriteLine(@"Its script for check mail by Imap.
    Usage: Imap.exe -s/--server *address:port* [--ssl] [-n begin end] -u *username*
        default port is 143
        -s/--server  -  imap server address and port
        --ssl   -  use ssl
        -n   -  letters range
        -u   -   mail user
        -p   -   user password
        ");
            Environment.Exit(1);
        }
    }
}