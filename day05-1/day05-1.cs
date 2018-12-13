using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day05_1 {
    class Program {
        static void Main(string[] args) {
            var inStream = new StreamReader(args[0]);
            string input = inStream.ReadLine();
            inStream.Dispose();
            char[] output = new char[input.Length];
            output[0] = input[0];
            int outputSize = 1;
            int caseDiff = Math.Abs('A' - 'a');
            for (int i = 1; i < input.Length; ++i) {
                if (outputSize > 0 && Math.Abs(output[outputSize-1] - input[i]) == caseDiff) {
                    outputSize -= 1;
                    continue;
                }
                output[outputSize] = input[i];
                outputSize += 1;
            }
            string outStr = new string(output, 0, outputSize);
            Console.WriteLine(outStr.Length);
        }
    }
}
