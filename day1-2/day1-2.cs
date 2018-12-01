using System;
using System.Collections.Generic;
using System.IO;

namespace day1_1 {
    class Program {
        static void Main(string[] args) {
            int freq = 0;
            var freqSet = new HashSet<int>();
            while (true) {
                var inStream = new StreamReader(args[0]);
                string line = "";
                while ((line = inStream.ReadLine()) != null) {
                    freq += Int32.Parse(line);
                    //Console.WriteLine($"freq = {freq}");
                    if (!freqSet.Add(freq)) {
                        Console.WriteLine($"first repeated freq = {freq}");
                        return;
                    }
                }
            }
        }
    }
}
