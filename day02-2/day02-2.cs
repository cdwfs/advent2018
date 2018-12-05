using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace day02_2 {
    class Program {
        static void Main(string[] args) {
            // Brute force ahoy!
            var inStream = new StreamReader(args[0]);
            string line = "";
            List<string> lines = new List<string>();
            while ((line = inStream.ReadLine()) != null) {
                foreach (string other in lines) {
                    Debug.Assert(other.Length == line.Length, "line length mismatch");
                    int diffCount = 0;
                    int diffIndex = -1;
                    for (int i = 0; i < line.Length; ++i) {
                        if (line[i] != other[i]) {
                            diffCount += 1;
                            diffIndex = i;
                        }
                    }
                    if (diffCount == 1) {
                        Console.WriteLine(line);
                        Console.WriteLine(other);
                        Console.WriteLine($"differ at index {diffIndex} ({line[diffIndex]} != {other[diffIndex]})");
                        Console.WriteLine($"common chars: {line.Substring(0, diffIndex)}{line.Substring(diffIndex + 1)}");
                        return;
                    }
                }
                lines.Add(line);
            }
            Console.WriteLine($"Found nothing?");
        }
    }
}
