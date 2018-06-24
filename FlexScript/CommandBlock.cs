using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
	public class CommandBlock {
		public List<string> list = new List<string>();
		public string[] array = null;
		public int length = -1;

		#region Constructors
		public CommandBlock() {
			// Create new empty command block
		}

		public CommandBlock(string[] lines) {
			// Set commandblock's fields and properties
			length = lines.Length;
			list = lines.ToList();
			array = lines;
		}

		public CommandBlock(List<string> lines) {
			// Set commandblock's fields and properties
			list = lines;
			array = lines.ToArray();
			length = array.Length;
		}

		public CommandBlock(int contexti, string[] context) {
			// Check for valid use of constructor
			if (context[contexti].EndsWith("(")) {
				// Move up
				contexti++;

				// Add lines to list of commands
				for (int i = contexti; i < context.Length; i++) {
					// Get line from context
					string line = context[i];

					if (line == ")") { // If end of commandblock, break
						break;
					} else { // Else, add line to command list
						list.Add(line);
					}
				}

				// Set commandblock's fields and properties
				array = list.ToArray();
				length = array.Length;
			} else {
				// This should never happen, unless the interpreter calls this specific constructor arbitrarily... somehow
				throw new Exception("error undefined");
			}
		}
		#endregion
	}
}
