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
			length = lines.ToArray().Length;
			array = lines.ToArray();
			list = lines;
		}
		#endregion
	}
}
