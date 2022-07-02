using System;
using System.Collections.Generic;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

namespace Imap
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLineOptions.ParseArgs(args);
            var password = options.password ?? ReadPasswordFromKeyboard();
            var client = new ImapClient();
            client.Connect(options.Address, options.port,
                options.ssl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);
            client.Authenticate(options.user, password);
            client.Inbox.Open(FolderAccess.ReadOnly);
            var headers = client
                .Inbox
                .Fetch(options.begin, options.end, MessageSummaryItems.Headers);
            foreach (var header in headers)
                Console.WriteLine(
                    $"To: {header.Headers["To"]} | From: {header.Headers["From"]} | Subject: {header.Headers["Subject"]}");
        }

        private static string ReadPasswordFromKeyboard()
        {
            var passwordChars = new Stack<char>();
            var key = new ConsoleKeyInfo();
            Console.Clear();
            Console.Write("Input password:");
            while (key.Key != ConsoleKey.Enter)
            {
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        continue;
                    case ConsoleKey.Backspace when passwordChars.Count > 0:
                        passwordChars.Pop();
                        Console.Write(' ');
                        Console.CursorLeft--;
                        break;
                    case ConsoleKey.Backspace:
                        Console.CursorLeft++;
                        break;
                    default:
                        if (char.IsLetterOrDigit(key.KeyChar))
                        {
                            passwordChars.Push(key.KeyChar);
                            Console.CursorLeft--;
                            Console.Write('*');
                        }
                        else
                            Console.CursorLeft--;
                        break;
                }
            }

            return new string(passwordChars.ToArray());
        }
    }
}