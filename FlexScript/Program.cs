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
						ParseLine(input, 0, new List<string>(), variables);
					} catch (Exception e) {
						// Output error
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\t$ | " + input);
						Console.WriteLine("\n\t" + e.Message);
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
			for (int i = 0; i < lines.ToArray().Length; i++) {
				// Get Line
				string line = lines.ToArray()[i];

				try {
					ParseLine(line, i, lines, variables);
				} catch (Exception e) {
					// Output error
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\t" + (i + 1) + " | " + line);
					Console.WriteLine("\n\t" + e.Message);
					Console.ForegroundColor = ConsoleColor.Gray;
					// Pause
					Console.ReadKey(true);
					// Stop parsing
					break;
				}
			}
		}

		static private void ParseLine(string line, int contexti, IEnumerable<string> context, Dictionary<string, string> vars) {
			Dictionary<string, string> variables = vars;

			// Removes comments from input line
			if (line.IndexOf("//") != -1)
				line = line.Substring(0, line.IndexOf("//"));

			// Formats input into tokens
			List<string> command = line.Split(' ').ToList();
			int commandLength = command.ToArray().Length;

			// Format command variables
			if (command[0] != "for") formatCommandVariables(command, commandLength, variables);

			#region Command parser
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

				#region for
				case "for":
					if (commandLength < 6) throw new Exception("Invalid Statement; not enough arguments for 'for'");

					// Get loop parameters
					string iterator = command[1];
					string type = command[2];
					string array = command[3];

					// For each token in result
					string result = "";
					foreach (string token in command.Skip(5)) {
						result += token + " ";
					}

					// Switch for type of loop
					switch (type) {
						case "times":
							// Get duration of loop
							int duration = int.Parse(array);
							// Add iterator to variables
							variables.Add(iterator, "0");

							for (int i = 0; i < duration; i++) {
								// Update iterator variable
								variables[iterator] = (i + 1).ToString();

								// Convert result to list
								List<string> resultCommand = result.Split(' ').ToList();
								// Format command variables
								formatCommandVariables(resultCommand, resultCommand.ToArray().Length, variables);

								// Convert list to string and parse
								ParseLine(string.Join(" ", resultCommand), contexti, context, variables);
							}

							// Remove iterator from variables
							variables.Remove(iterator);
							break;

						default:
							throw new Exception("Invalid Statement; unsupported comparator used in 'if'");
					}
					break;
				#endregion

				#region if
				case "if":
					if (commandLength < 6) throw new Exception("Invalid Statement; not enough arguments for 'if'");

					// Create method to get state of lexxing
					int lexStateIf = 0;

					// Create lexxing outputs
					List<string> original = new List<string>();
					string comparator = "";
					List<string> compare = new List<string>();
					string ifResult = "";

					// For each token in statement
					foreach (string token in command.Skip(1)) {
						if (lexStateIf == 0) { // While lexxing first half of statement
							if (getComprarators().Contains(token)) { // If done, go to next half
								comparator = token;
								lexStateIf++;
							} else { // If not done, add tokens to original
								original.Add(token);
							}
						} else if (lexStateIf == 1) { // While lexxing second half of statement
							if (token == "then") { // If done, go to result
								lexStateIf++;
							} else { // If not done, add tokens to compare
								compare.Add(token);
							}
						} else if (lexStateIf == 2) { // While lexxing result, add tokens to result
							ifResult += token + " ";
						}
					}

					// Switch for type of comparators
					switch (comparator) {
						case "==":
							// If 'equals' compratator, check if original and compare are equal
							if (original.SequenceEqual(compare)) ParseLine(ifResult, contexti, context, variables);
							break;

						case "!=":
							// If 'not equals' compratator, check if original and compare are not equal
							if (!original.SequenceEqual(compare)) ParseLine(ifResult, contexti, context, variables);
							break;

						case "<":
							int lessX = int.Parse(string.Join("", original));
							int lessY = int.Parse(string.Join("", compare));
							if (lessX < lessY) ParseLine(ifResult, contexti, context, variables);
							break;

						case ">":
							int greatX = int.Parse(string.Join("", original));
							int greatY = int.Parse(string.Join("", compare));
							if (greatX > greatY) ParseLine(ifResult, contexti, context, variables);
							break;

						default:
							throw new Exception("Invalid Statement; unsupported comparator used in 'if'");
					}
					break;
				#endregion

				#region var
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
							int addSum = int.Parse(command[3]);

							if (commandLength == 4) {
								// Add to current variable's content
								addSum += int.Parse(variables[command[1]]);
							} else {
								// For each numeral to add
								foreach (string value in command.Skip(4)) {
									// Convert value to number and add it to total
									addSum += int.Parse(value);
								}
							}

							// Set variable to total
							variables[command[1]] = addSum.ToString();
							break;

						case "-=":
							// Create total result
							int minSum = int.Parse(command[3]);

							if (commandLength == 4) {
								// Take from current variable's content by number
								minSum -= int.Parse(variables[command[1]]);
							} else {
								// For each numeral to subtract
								foreach (string value in command.Skip(4)) {
									// Convert value to number and subtract it from total
									minSum -= int.Parse(value);
								}
							}

							// Set variable to total
							variables[command[1]] = minSum.ToString();
							break;

						case "*=":
							// Create total result
							int multSum = int.Parse(command[3]);

							if (commandLength == 4) {
								// Multiply by current variable's content
								multSum *= int.Parse(variables[command[1]]);
							} else {
								// For each numeral to multiply
								foreach (string value in command.Skip(4)) {
									// Convert value to number and multiply it by total
									multSum *= int.Parse(value);
								}
							}

							// Set variable to total
							variables[command[1]] = multSum.ToString();
							break;

						case "/=":
							// Create total result
							int divSum = int.Parse(command[3]);

							if (commandLength == 4) {
								// Divide current variable's content by number
								divSum /= int.Parse(variables[command[1]]);
							} else {
								// For each numeral to divide
								foreach (string value in command.Skip(4)) {
									// Convert value to number and divide the total by it
									divSum /= int.Parse(value);
								}
							}

							// Set variable to total
							variables[command[1]] = divSum.ToString();
							break;

						default:
							throw new Exception("Invalid Token; unsupported variable assignment token");
					}
					break;
				#endregion

				default:
					throw new Exception("Invalid Token; invalid command '" + command[0] + "'");
			}
			#endregion
		}
		#endregion

		#region Helper Functions
		static private void formatCommandVariables(List<string> command, int commandLength, Dictionary<string, string> variables) {
			try {
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
			} catch (Exception e) {
				if (e.Message == "The given key was not present in the dictionary.") throw new Exception("Invalid Token; variable doesn't exist");
				
				throw e;
			}
		}

		static private void printMetadataHeader() {
			// Draw metadata header
			Console.ForegroundColor = ConsoleColor.Green; Console.Write("\n" + Environment.UserName.ToLower() + "@FlexScript ");
			Console.ForegroundColor = ConsoleColor.Magenta; Console.Write("FLXVR30 ");
			Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("~/Minin/Zvava ");
			Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("(MIT)");

			// Reset foreground color
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		static private List<string> getComprarators() {
			return new List<string>() {
				"==",
				"!=",
				"<",
				">"
			};
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
