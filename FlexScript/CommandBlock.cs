using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
	public class CommandBlock {
		List<string> list;
		string[] array;
		int length;

		#region Constructors
		public CommandBlock(string[] lines) {
			length = lines.Length;
			list = lines.ToList();
			array = lines;
		}

		public CommandBlock(List<string> lines) {
			list = lines;
			array = lines.ToArray();
			length = array.Length;
		}

		public CommandBlock(int contexti, string[] context) {
			if (context[contexti].EndsWith("(")) {
				for (int i = contexti; i < context.Length; i++) {
					string line = context[i];

					if (line == ")") {
						break;
					} else {
						list.Add(line);
					}
				}

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
