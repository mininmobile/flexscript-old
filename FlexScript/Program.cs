using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlexScript {
	class Program {
		static void Main(string[] args) {
			// Create interpreter
			Interpreter interpreter = new Interpreter();

			// Set title
			Console.Title = "FLXVR30:/c/Minin/Zvava";

			// Parse command-line arguments
			if (args.Length == 0) {
				// Launch Console
				bool running = true;
				while (running) {
					// Draw metadata header
					PrintMetadataHeader();

					// Get user input
					Console.Write("\n$ ");
					string input = Console.ReadLine();

					// Parse user input
					try {
						interpreter.ParseLine(input, new List<string>());
					} catch (Exception e) {
						if (e.Message == "EC0") running = false;

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
						interpreter.ParseFile(args[0]);
					} else {
						throw new Exception("Invalid Arguments; invalid file specified");
					}
				} else {
					throw new Exception("Invalid Arguments; asked to run file without specifying file");
				}
			} else if (File.Exists(args[0])) {
				// Parse file
				interpreter.ParseFile(args[0]);
			} else {
				throw new Exception("Invalid Arguments");
			}
		}

		#region Helper Functions
		static private void PrintMetadataHeader() {
			// Draw metadata header
			Console.ForegroundColor = ConsoleColor.Green; Console.Write("\n" + Environment.UserName.ToLower() + "@FlexScript ");
			Console.ForegroundColor = ConsoleColor.Magenta; Console.Write("FLXVR30 ");
			Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("~/Minin/Zvava ");
			Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("(MIT)");

			// Reset foreground color
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		#endregion
	}
}
