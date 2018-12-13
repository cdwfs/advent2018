using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day06_2 {
    class Program {
        const int MAXSUM = 10000;
        //const int MAXSUM = 32;
        const int DIM = 360 + 2;
        //const int DIM = 10 + 2;
        
        static void Main(string[] args) {
            Regex rx = new Regex(@"^(?<x>\d+), (?<y>\d+)", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            int lineCount = 0;
            string[] testLines = {
                "1, 1",
                "1, 6",
                "8, 3",
                "3, 4",
                "5, 5",
                "8, 9",
            };
            var points = new List<(int, int)>();
            while ((line = inStream.ReadLine()) != null) {
            //foreach (var line in testLines) {
                var m = rx.Match(line);
                int x = Int32.Parse(m.Groups["x"].Value);
                int y = Int32.Parse(m.Groups["y"].Value);
                points.Add((x+1, y+1));
                lineCount += 1;
            }

            int safeCellCount = 0;
            for (int y = 1; y < DIM - 1; ++y) {
                for (int x = 1; x < DIM - 1; ++x) {
                    int distance = 0;
                    foreach (var (px,py) in points) {
                        distance += Math.Abs(px - x) + Math.Abs(py - y);
                    }
                    if (distance < MAXSUM) {
                        safeCellCount += 1;
                        //Console.Write("#");
                    } else {
                        //Console.Write(".");
                    }
                }
                //Console.Write("\n");
            }

            Console.WriteLine($"{safeCellCount} safe cells.");
        }
    }
}
