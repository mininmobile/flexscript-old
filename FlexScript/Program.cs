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
                        Console.WriteLine(RunFile(args[0]));
                    } else {
                        throw new Exception("Invalid Arguments; invalid file specified");
                    }
                } else {
                    throw new Exception("Invalid Arguments; asked to run file without specifying file");
                }
            } else if (File.Exists(args[0])) {
                Console.WriteLine(RunFile(args[0]));
            } else {
                throw new Exception("Invalid Arguments");
            }

            Console.ReadKey();
        }

        static private string RunFile(string filePath) {
            IEnumerable<string> lines = File.ReadLines(filePath);

            string output = "";

            output = string.Join(", ", lines);

            return output;
        }
    }
}
