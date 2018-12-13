using System;
using System.IO;

namespace day01_1 {
    class Program {
        static void Main(string[] args) {
            var inStream = new StreamReader(args[0]);
            string line = "";
            int freq = 0;
            while ((line = inStream.ReadLine()) != null) {
                freq += Int32.Parse(line);
            }
            inStream.Dispose();
            Console.WriteLine($"freq = {freq}");
        }
    }
}
