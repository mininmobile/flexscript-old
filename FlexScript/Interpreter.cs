using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
	class Interpreter {
		Dictionary<string, string> variables = new Dictionary<string, string>();
		Dictionary<string, int> labels = new Dictionary<string, int>();
		int contexti = 0;

		public Interpreter() { }

		public Interpreter(Dictionary<string, string> vars) {
			variables = vars;
		}

		#region Parser Functions
		public void ParseFile(string filePath) {
			// Read all input lines from file
			IEnumerable<string> lines = File.ReadLines(filePath);
			int llines = lines.ToArray().Length;

			// Get all labels
			// Prepare lines
			for (int i = 0; i < llines; i++) {
				// Get Line
				string line = lines.ToArray()[i];
				string[] command = line.Split(' ');

				// If line is a label
				if (line.StartsWith(":")) {
					labels[command[0].Substring(1, line.Length - 1)] = i;
				}
			}

			// Parse each input line in file
			for (contexti = 0; contexti < llines; contexti++) {
				// Get Line
				string line = lines.ToArray()[contexti];

				try {
					ParseLine(line, lines);
				} catch (Exception e) {
					// Output error
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\t" + (contexti + 1) + " | " + line);
					Console.WriteLine("\n\t" + e.Message);
					Console.ForegroundColor = ConsoleColor.Gray;
					// Pause
					Console.ReadKey(true);
					// Stop parsing
					break;
				}
			}
		}

		public void ParseLine(string line, IEnumerable<string> context) {
			// Formats input into tokens
			List<string> command = line.Split(' ').ToList();
			int commandLength = command.ToArray().Length;

			// Format indentation
			command[0] = command[0].Replace("\t", "");

			// Format command variables
			if (!(new string[] { "try", "for" }.Contains(command[0])))
				FormatCommandVariables(command, commandLength);

			#region Command parser
			if (command[0] != "" && !command[0].StartsWith(":")) switch (command[0]) {
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
						Console.BackgroundColor = GetColors()[command[1]];
						break;

					case "color":
						if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

						// Change console foregrund color
						Console.ForegroundColor = GetColors()[command[1]];
						break;

					case "call":
						if (commandLength == 1) throw new Exception("Invalid Token; command expected argument");

						Interpreter call = new Interpreter(variables);

						call.ParseFile(string.Join(" ", command.Skip(1)));

						variables = call.variables;
						break;

					case "goto":
						if (commandLength < 2) throw new Exception("Invalid Token; command expected argument");

						try {
							contexti = labels[command[1]];
						} catch {
							throw new Exception("Invalid Token; label doesn't exist");
						}
						break;

					case "throw":
						if (commandLength < 2)
							throw new Exception("Invalid Token; command expected argument");

						// Get error message
						string message = string.Join(" ", command.Skip(2));

						// Get error type
						string error = "Error; ";
						switch (command[1]) {
							case "error":
								error = "Error; ";
								break;
							case "invalidtokenerror":
								error = "Invalid Token; ";
								break;
							case "invalidstatementerror":
								error = "Invalid Statement; ";
								break;

							default:
								throw new Exception("Invalid Token; unsupported error used in 'throw'");
						}

						// Throw error
						if (message.Length == 0)
							throw new Exception(error + "undefined");
						else
							throw new Exception(error + message);
						break;

					#region file
					case "file":
						if (commandLength < 3) throw new Exception("Invalid Token; command expected argument");

						switch (command[1]) {
							case "get":
								// Get file content
								string[] lines = File.ReadLines(string.Join(" ", command.Skip(2))).ToArray();
								// Create file variable
								variables["file"] = "[ file:buffer ]";
								// Create lines variable
								variables["file.lines"] = string.Join(",", lines);
								// Create length variable
								variables["file.length"] = lines.Length.ToString();
								// Create item variables
								for (int i = 0; i < lines.Length; i++) {
									variables["file.lines[" + i + "]"] = lines[i];
								}
								break;

							case "exists":
								// Create file variable
								variables["file"] = "[ io:stream ]";
								// Create existance variable
								variables["file.exists"] = File.Exists(string.Join(" ", command.Skip(2))).ToString().ToLower();
								break;

							case "copy":
								// Get string locations
								string[] location = string.Join(" ", command.Skip(2)).Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);

								// Copy file
								File.Copy(location[0], location[1]);
								break;

							case "delete":
								// Remove specified file
								File.Delete(string.Join(" ", command.Skip(2)));
								break;

							case "append":
								// Create lex state
								int lexStateAppend = 0;

								// Create lex outputs
								List<string> contentSplitAppend = new List<string>();
								string[] contentLinesAppend;
								string contentRawAppend = "";
								List<string> resultArrayAppend = new List<string>();
								string resultFileAppend = "";

								foreach (string token in command.Skip(2)) {
									if (lexStateAppend == 0) {
										if (token == "to") {
											lexStateAppend++;
										} else {
											contentSplitAppend.Add(token);
										}
									} else if (lexStateAppend == 1) {
										resultArrayAppend.Add(token);
									}
								}

								resultFileAppend = string.Join(" ", resultArrayAppend);
								contentRawAppend = string.Join(" ", contentSplitAppend);
								contentLinesAppend = contentRawAppend.Split(',');

								File.AppendAllLines(resultFileAppend, contentLinesAppend);
								break;

							case "write":
								// Create lex state
								int lexStateWrite = 0;

								// Create lex outputs
								List<string> contentSplitWrite = new List<string>();
								string[] contentLinesWrite;
								string contentRawWrite = "";
								List<string> resultArrayWrite = new List<string>();
								string resultFileWrite = "";

								foreach (string token in command.Skip(2)) {
									if (lexStateWrite == 0) {
										if (token == "to") {
											lexStateWrite++;
										} else {
											contentSplitWrite.Add(token);
										}
									} else if (lexStateWrite == 1) {
										resultArrayWrite.Add(token);
									}
								}

								resultFileWrite = string.Join(" ", resultArrayWrite);
								contentRawWrite = string.Join(" ", contentSplitWrite);
								contentLinesWrite = contentRawWrite.Split(',');

								File.WriteAllLines(resultFileWrite, contentLinesWrite);
								break;

							default:
								throw new Exception("Invalid Token; unsupported file operation '" + command[1] + "' used in 'file'");
						}
						break;
					#endregion

					#region try
					case "try":
						// Create method to get state of lexxing
						int lexStateTry = 0;

						// Create lexxing outputs
						List<string> tryArray = new List<string>();
						string tryResult = "";
						List<string> catchArray = new List<string>();
						string catchResult = "";

						// For each token in statement
						foreach (string token in command.Skip(1)) {
							if (lexStateTry == 0) { // While lexxing first half
								if (token == "catch") {
									lexStateTry++;
								} else { // While lexxing second half
									tryArray.Add(token);
								}
							} else if (lexStateTry == 1) {
								catchArray.Add(token);
							}
						}

						tryResult = string.Join(" ", tryArray);
						catchResult = string.Join(" ", catchArray);

						try {
							ParseLine(tryResult, context);
						} catch (Exception e) {
							variables["e"] = e.Message;
							ParseLine(catchResult, context);
							variables.Remove("e");
						}
						break;
					#endregion

					#region for
					case "for":
						if (commandLength < 6) throw new Exception("Invalid Statement; not enough arguments for 'for'");

						// Create method to get state of lexxing
						int lexStateFor = 0;

						// Create lexxing outputs
						string iterator = command[1];
						string type = "";
						List<string> array = new List<string>();
						string forResult = "";

						// Format Variables on Loop
						FormatCommandVariables(command, commandLength, iterator);

						// For each token in statement
						foreach (string token in command.Skip(1)) {
							if (lexStateFor == 0) { // While lexxing first half of statement
								if (new string[] { "times", "in" }.Contains(token)) { // If done, go to next half
									type = token;
									lexStateFor++;
								}
							} else if (lexStateFor == 1) { // While lexxing second half of statement
								if (token == "then") { // If done, go to result
									lexStateFor++;
								} else { // If not done, add tokens to array
									array.Add(token);
								}
							} else if (lexStateFor == 2) { // While lexxing result, add tokens to result
								forResult += token + " ";
							}
						}

						// Switch for type of loop
						switch (type) {
							case "times":
								// Get duration of loop
								int duration = int.Parse(string.Join("", array));
								// Add iterator to variables
								variables[iterator] = "0";

								for (int i = 0; i < duration; i++) {
									// Update iterator variable
									variables[iterator] = (i + 1).ToString();

									// Convert result to list
									List<string> resultCommand = forResult.Split(' ').ToList();
									// Format command variables
									FormatCommandVariables(resultCommand, resultCommand.ToArray().Length);

									// Convert list to string and parse
									ParseLine(string.Join(" ", resultCommand), context);
								}

								// Remove iterator from variables
								variables.Remove(iterator);
								break;

							case "in":
								// Get array
								string[] arr = string.Join(" ", array).Split(',');
								// Add iterator to variables
								variables[iterator] = "";

								for (int i = 0; i < arr.Length; i++) {
									// Update iterator variable
									variables[iterator] = arr[i];

									// Convert result to list
									List<string> resultCommand = forResult.Split(' ').ToList();
									// Format command variables
									FormatCommandVariables(resultCommand, resultCommand.ToArray().Length);

									// Convert list to string and parse
									ParseLine(string.Join(" ", resultCommand), context);
								}

								// Remove iterator from variables
								variables.Remove(iterator);
								break;

							default:
								throw new Exception("Invalid Statement; unsupported comparator used in 'for'");
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
								if (GetComprarators().Contains(token)) { // If done, go to next half
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
								// Are original and compare are equal
								if (original.SequenceEqual(compare)) ParseLine(ifResult, context);
								break;

							case "!=":
								// Are original and compare not equal
								if (!original.SequenceEqual(compare)) ParseLine(ifResult, context);
								break;

							case "<":
								// Convert original and compare to numbers
								int lessX = int.Parse(string.Join("", original));
								int lessY = int.Parse(string.Join("", compare));
								// Is original or compare lesser
								if (lessX < lessY) ParseLine(ifResult, context);
								break;

							case ">":
								// Convert original and compare to numbers
								int greatX = int.Parse(string.Join("", original));
								int greatY = int.Parse(string.Join("", compare));
								// Is original or compare greater
								if (greatX > greatY) ParseLine(ifResult, context);
								break;

							case "<=":
								// Convert original and compare to numbers
								int lessEqualX = int.Parse(string.Join("", original));
								int lessEqualY = int.Parse(string.Join("", compare));
								// Is original or compare lesser/equal
								if (lessEqualX <= lessEqualY) ParseLine(ifResult, context);
								break;

							case ">=":
								// Convert original and compare to numbers
								int greatEqualX = int.Parse(string.Join("", original));
								int greatEqualY = int.Parse(string.Join("", compare));
								// Is original or compare greater/equal
								if (greatEqualX >= greatEqualY) ParseLine(ifResult, context);
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

							case "=[]":
								// Create array variable
								variables[command[1]] = string.Join(" ", command.Skip(3));
								// Create length variable
								variables[command[1] + ".length"] = variables[command[1]].Split(',').Length.ToString();
								// Create item variables
								for (int i = 0; i < variables[command[1]].Split(',').Length; i++) {
									variables[command[1] + "[" + i + "]"] = variables[command[1]].Split(',')[i];
								}
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
		public void FormatCommandVariables(List<string> command, int commandLength, string ignore = "") {
			try {
				// Formats variables into variable contents
				for (int i = 0; i < commandLength; i++) {
					// Get token
					string token = command[i];

					// Get Closing Bracket
					int close = token.IndexOf("}");
					// If token matches variables criterea
					if (token.StartsWith("{") && close != -1) {
						if (token.Substring(1, close - 1) != ignore) {
							// Replace variable placeholder with variable's contents
							command[i] = variables[token.Substring(1, close - 1)] + token.Substring(close + 1, token.Length - close - 1);
						}
					}
				}
			} catch (Exception e) {
				if (e.Message == "The given key was not present in the dictionary.") throw new Exception("Invalid Token; variable doesn't exist");

				throw e;
			}
		}

		public List<string> GetComprarators() {
			return new List<string>() {
				"==",
				"!=",
				"<",
				">",
				"<=",
				">="
			};
		}

		public Dictionary<string, ConsoleColor> GetColors() {
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
