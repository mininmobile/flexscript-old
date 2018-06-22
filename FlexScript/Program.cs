using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
    class Program {
        static void Main(string[] args) {
            // Create variables container
            Dictionary<string, string> variables = new Dictionary<string, string>();

            // Set title
            Console.Title = "FLXVR30:/c/Minin/Zvava";

            // Parse command-line arguments
            if (args.Length == 0) {
                // Launch Console
                bool running = true;
                while (running) {
                    // Draw metadata header
                    printMetadataHeader();
                    
                    // Get user input
                    Console.Write("\n$ ");
                    string input = Console.ReadLine();

                    // Parse user input
                    try {
                        ParseLine(input, variables);
                    } catch (Exception e) {
                        // Output error
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\t" + e.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            } else if (args[0] == "-f" || args[0] == "--file") {
                // Check for sufficient arguments
                if (args.Length > 1) {
                    if (File.Exists(args[1])) {
                        // If file exists, parse file
                        ParseFile(args[0], variables);
                    } else {
                        throw new Exception("Invalid Arguments; invalid file specified");
                    }
                } else {
                    throw new Exception("Invalid Arguments; asked to run file without specifying file");
                }
            } else if (File.Exists(args[0])) {
                // Parse file
                ParseFile(args[0], variables);
            } else {
                throw new Exception("Invalid Arguments");
            }
        }

        #region Parser Functions
        static private void ParseFile(string filePath, Dictionary<string, string> vars) {
            Dictionary<string, string> variables = vars;

            // Read all input lines from file
            IEnumerable<string> lines = File.ReadLines(filePath);

            // Parse each input line in file
            foreach (string line in lines) {
                try {
                    ParseLine(line, variables);
                } catch (Exception e) {
                    // Output error
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\t" + e.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    // Pause
                    Console.ReadKey(true);
                    // Stop parsing
                    break;
                }
            }
        }

        static private void ParseLine(string line, Dictionary<string, string> vars) {
            Dictionary<string, string> variables = vars;

            // Removes comments from input line
            if (line.IndexOf("//") != -1)
                line = line.Substring(0, line.IndexOf("//"));

            // Formats input into tokens
            List<string> command = line.Split(' ').ToList();
            int commandLength = command.ToArray().Length;

            // Formats variables into variable contents
            for (int i = 0; i < commandLength; i++) {
                // Get token
                string token = command[i];

                // Get Closing Bracket
                int close = token.IndexOf("}");
                // If token matches variables criterea
                if (token.StartsWith("{") && close != -1) {
                    // Replace variable placeholder with variable's contents
                    command[i] = variables[token.Substring(1, close - 1)] + token.Substring(close + 1, token.Length - close - 1);
                }
            }

            // Command parser
            switch (command[0]) {
                case "print":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

                    // Output the rest of the line
                    Console.WriteLine(string.Join(" ", command.Skip(1)));
                    break;

                case "pause":
                    if (commandLength == 1) { // If given no arguments, pause with output
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    } else if (commandLength == 2 && command[1] == "-s" || command[1] == "--silent") { // If given silent argument, pause with no output
                        Console.ReadKey(true);
                    } else if (commandLength > 2 && command[1] == "-c" || command[1] == "--custom") { // If given custom argument, pause with custom output
                        Console.WriteLine(string.Join(" ", command.Skip(2)));
                        Console.ReadKey(true);
                    } else { // This should neven happen lol
                        throw new Exception("error undefined");
                    }
                    break;

                case "clear":
                    // Clear console
                    Console.Clear();
                    break;
                    
                case "var":
                    if (commandLength < 3) throw new Exception("Invalid Token; command expected arguments");

                    switch (command[2]) {
                        case "=":
                            // Set variable to value
                            variables[command[1]] = string.Join(" ", command.Skip(3));
                            break;

                        case "<=":
                            // Output question
                            Console.Write(string.Join(" ", command.Skip(3)));
                            // Set variable to input
                            variables[command[1]] = Console.ReadLine();
                            break;

                        case "<":
                            // Set variable to input
                            variables[command[1]] = Console.ReadLine();
                            break;

                        case "+=":
                            // Create total result
                            int total = 0;
                            // For each numeral to add
                            foreach (string value in command.Skip(3)) {
                                // Convert value to number and add it to total
                                total += int.Parse(value);
                            }

                            // Set variable to total
                            variables[command[1]] = total.ToString();
                            break;

                        default:
                            throw new Exception("Invalid Token; unsupported variable assignment token");
                    }
                    break;

                case "background":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

                    // Change console color
                    Console.BackgroundColor = getColors()[command[1]];
                    break;

                case "color":
                    if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

                    // Change console foregrund color
                    Console.ForegroundColor = getColors()[command[1]];
                    break;

                default:
                    throw new Exception("Invalid Token; invalid command '" + command[0] + "'");
            }
        }
        #endregion

        #region Helper Functions
        static private void printMetadataHeader() {
            // Draw metadata header
            Console.ForegroundColor = ConsoleColor.Green; Console.Write("\n" + Environment.UserName.ToLower() + "@FlexScript ");
            Console.ForegroundColor = ConsoleColor.Magenta; Console.Write("FLXVR30 ");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("~/Minin/Zvava ");
            Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("(MIT)");

            // Reset foreground color
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static private Dictionary<string, ConsoleColor> getColors() {
            // Return dictionary of the 16 supported terminal colors
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
