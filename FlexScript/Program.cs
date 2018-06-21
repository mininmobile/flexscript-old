using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Launched FlexScript Console");
            } else if (args[0] == "-f" || args[0] == "--file") {
                if (args.Length > 1) {
                    if (File.Exists(args[1])) {
                        ParseFile(args[0]);
                    } else {
                        throw new Exception("Invalid Arguments; invalid file specified");
                    }
                } else {
                    throw new Exception("Invalid Arguments; asked to run file without specifying file");
                }
            } else if (File.Exists(args[0])) {
                ParseFile(args[0]);
            } else {
                throw new Exception("Invalid Arguments");
            }

            Console.ReadKey();
        }

        static private void ParseFile(string filePath) {
            IEnumerable<string> lines = File.ReadLines(filePath);

            foreach (string line in lines) {
                ParseLine(line);
            }
        }

        static private void ParseLine(string line) {
            string[] command = line.Split(' ');

            switch (command[0]) {
                case "print":
                    if (command.Length == 1) throw new Exception("Invalid Token; command expected argument");

                    string printable = string.Join(" ", command.Skip(1));
                    Console.WriteLine(printable);
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "background":
                    if (command.Length == 1) throw new Exception("Invalid Token; command expected argument");

                    Console.BackgroundColor = getColors()[command[1]];
                    break;

                case "color":
                    if (command.Length == 1) throw new Exception("Invalid Token; command expected argument");

                    Console.ForegroundColor = getColors()[command[1]];
                    break;

                default:
                    throw new Exception("Invalid Token; invalid command '" + command[0] + "'");
            }
        }

        #region Helper Functions
        static private Dictionary<string, ConsoleColor> getColors() {
            return new Dictionary<string, ConsoleColor>() {
                {"red", ConsoleColor.Red},
                {"darkred", ConsoleColor.DarkRed},
                {"green", ConsoleColor.Green},
                {"darkgreen", ConsoleColor.DarkGreen},
                {"blue", ConsoleColor.Blue},
                {"cyan", ConsoleColor.Cyan},
                {"darkcyan", ConsoleColor.DarkCyan},
                {"darkblue", ConsoleColor.DarkBlue},
                {"magenta", ConsoleColor.Magenta},
                {"darkmagenta", ConsoleColor.DarkMagenta},
                {"yellow", ConsoleColor.Yellow},
                {"darkyellow", ConsoleColor.DarkYellow},
                {"white", ConsoleColor.White},
                {"gray", ConsoleColor.Gray},
                {"darkgray", ConsoleColor.DarkGray},
                {"black", ConsoleColor.Black}
            };
        }
        #endregion
    }
}
