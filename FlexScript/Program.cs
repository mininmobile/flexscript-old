using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexScript {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Launched FlexScript Console");
            } else {
                if (args[0] == "-f" || args[0] == "--file") {
                    Console.WriteLine("Launched FlexScript Interpreter to File");
                } else {
                    Console.WriteLine("fatal: invalid arguments");
                }
            }

            Console.ReadKey();
        }
    }
}
