using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
    class Program {
        static void Main(string[] args) {
            Dictionary<string, string> variables = new Dictionary<string, string>();

            if (args.Length == 0) {
                Console.WriteLine("Launched FlexScript Console");
            } else if (args[0] == "-f" || args[0] == "--file") {
                if (args.Length > 1) {
                    if (File.Exists(args[1])) {
                        ParseFile(args[0], variables);
                    } else {
                        throw new Exception("Invalid Arguments; invalid file specified");
                    }
                } else {
                    throw new Exception("Invalid Arguments; asked to run file without specifying file");
                }
            } else if (File.Exists(args[0])) {
                ParseFile(args[0], variables);
            } else {
                throw new Exception("Invalid Arguments");
            }

            Console.ReadKey();
        }

        static private void ParseFile(string filePath, Dictionary<string, string> vars) {
            IEnumerable<string> lines = File.ReadLines(filePath);
            Dictionary<string, string> variables = vars;

            foreach (string line in lines) {
                ParseLine(line, variables);
            }
        }

        static private void ParseLine(string line, Dictionary<string, string> vars) {
            Dictionary<string, string> variables = vars;
            List<string> command = line.Split(' ').ToList();
            int commandLength = command.ToArray().Length;

            for (int i = 0; i < commandLength; i++) {
                string token = command[i];
                int close = token.IndexOf("}");
                if (token.Contains("{") && close != -1) {
                    command[i] = variables[token.Substring(1, close - 1)] + token.Substring(close + 1, token.Length - close - 1);
                }
            }

            switch (command[0]) {
                case "print":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

                    Console.WriteLine(string.Join(" ", command.Skip(1)));
                    break;

                case "pause":
                    if (commandLength == 1) {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    } else if (commandLength == 2 && command[1] == "-s" || command[1] == "--silent") {
                        Console.ReadKey(true);
                    } else if (commandLength > 2 && command[1] == "-c" || command[1] == "--custom") {
                        Console.WriteLine(string.Join(" ", command.Skip(2)));
                        Console.ReadKey(true);
                    } else {
                        throw new Exception("error undefined");
                    }
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "var":
                    if (commandLength < 3) throw new Exception("Invalid Token; command expected arguments");

                    switch (command[2]) {
                        case "=":
                            if (variables.Keys.Contains(command[1]))
                                variables[command[1]] = string.Join(" ", command.Skip(3));
                            else
                                variables.Add(command[1], string.Join(" ", command.Skip(3)));
                            break;

                        case "<=":
                            Console.Write(string.Join(" ", command.Skip(3)));
                            if (variables.Keys.Contains(command[1]))
                                variables[command[1]] = Console.ReadLine();
                            else
                                variables.Add(command[1], Console.ReadLine());
                            break;

                        case "<":
                            if (variables.Keys.Contains(command[1]))
                                variables[command[1]] = Console.ReadLine();
                            else
                                variables.Add(command[1], Console.ReadLine());
                            break;

                        default:
                            throw new Exception("Invalid Token; unsupported variable assignment token");
                    }
                    break;

                case "background":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

                    Console.BackgroundColor = getColors()[command[1]];
                    break;

                case "color":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

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
