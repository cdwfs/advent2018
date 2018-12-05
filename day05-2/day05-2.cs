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
            int caseDiff = Math.Abs('A' - 'a');
            for (char c = 'a'; c <= 'z'; ++c) {
                char upper = Char.ToUpper(c);
                char[] output = new char[input.Length];
                int inputStart = 0;
                do {
                    output[0] = input[inputStart];
                    inputStart += 1;
                } while (output[0] == c || output[0] == upper);
                int outputSize = 1;
                for (int i = inputStart; i < input.Length; ++i) {
                    if (input[i] == c || input[i] == upper) {
                        continue;
                    }
                    if (outputSize > 0 && Math.Abs(output[outputSize - 1] - input[i]) == caseDiff) {
                        outputSize -= 1;
                        continue;
                    }
                    output[outputSize] = input[i];
                    outputSize += 1;
                }
                string outStr = new string(output, 0, outputSize);
                Console.WriteLine($"without {c}, length = {outStr.Length}");
            }
        }
    }
}
