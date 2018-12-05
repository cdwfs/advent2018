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
            var outputs = new char[26][];
            for (int c = 0; c < 26; ++c) {
                outputs[c] = new char[input.Length];
            }
            var outputSizes = new int[26];
            for (int i = 0; i < input.Length; ++i) {
                for (int c = 0; c < 26; ++c) {
                    char lower = (char)('a' + c);
                    char upper = (char)('A' + c);
                    if (input[i] == lower || input[i] == upper) {
                        continue;
                    }
                    if (outputSizes[c] > 0 && Math.Abs(outputs[c][outputSizes[c] - 1] - input[i]) == caseDiff) {
                        outputSizes[c] -= 1;
                    } else {
                        outputs[c][outputSizes[c]] = input[i];
                        outputSizes[c] += 1;
                    }
                }
            }

            for (int c = 0; c < 26; ++c) {
                string outStr = new string(outputs[c], 0, outputSizes[c]);
                Console.WriteLine($"without {(char)('a' + c)}, length = {outStr.Length}");
            }
        }
    }
}
