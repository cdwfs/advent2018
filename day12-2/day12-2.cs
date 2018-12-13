using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day12_2 {
    class Program {
        static void Main(string[] args) {
            const int MAX_GENERATION = 20;
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            // Read initial pot state
            string pots;
            int indexOffset = 0;
            {
                Regex rx0 = new Regex(@"^initial state: (?<state>[#.]+)", RegexOptions.Compiled);
                line = inStream.ReadLine();
                var match = rx0.Match(line);
                string state = match.Groups["state"].Value; // TIL: Value

                // find first/last full pot
                int firstFull = state.IndexOf('#');
                int lastFull = state.LastIndexOf('#');
                // construct padded array with room for "...." on each end of the valid range
                int potCount = (lastFull - firstFull) + 9;
                char[] next = new char[potCount];
                next[0] = next[1] = next[2] = next[3] = '.';
                next[potCount - 4] = next[potCount - 3] = next[potCount - 2] = next[potCount - 1] = '.';
                state.CopyTo(firstFull, next, 4, lastFull - firstFull + 1);
                // Keep track of which element the first element of the new array corresponds to
                indexOffset = indexOffset + firstFull - 4;
                // convert back to string
                pots = new string(next);
                
                line = inStream.ReadLine(); // blank line
                Console.WriteLine($"{0,2:D} {indexOffset,3:D}: {pots}");
            }

            // Read rules
            Regex rx = new Regex(@"^(?<pattern>[#.]{5}) => (?<result>[#.])", RegexOptions.Compiled);
            var rules = new Dictionary<string, char>();
            while ((line = inStream.ReadLine()) != null) {
                var match = rx.Match(line);
                string pattern = match.Groups["pattern"].Value;
                char result = Char.Parse(match.Groups["result"].Value);
                rules.Add(pattern, result);
            }
            inStream.Dispose(); // TIL: Dispose

            // simulate!
            var gens = new Dictionary<string, int>();
            int g = 0;
            while (true) {
                g += 1;
                // generate next generation of pots
                char[] next = new char[pots.Length];
                next[0] = next[1] = '.';
                next[pots.Length - 2] = next[pots.Length - 1] = '.';
                for (int i = 0; i < next.Length - 4; ++i) {
                    next[i + 2] = rules[pots.Substring(i, 5)];
                }
                // Find first/last full pot
                int firstFull = Array.FindIndex(next, p => p == '#');
                int lastFull = Array.FindLastIndex(next, p => p == '#');
                // construct padded array with room for "...." on each end of the valid range
                int newPotCount = (lastFull - firstFull) + 9;
                char[] newPots = new char[newPotCount];
                newPots[0] = newPots[1] = newPots[2] = newPots[3] = '.';
                newPots[newPotCount - 4] = newPots[newPotCount - 3] = newPots[newPotCount - 2] = newPots[newPotCount - 1] = '.';
                for (int i = firstFull; i <= lastFull; ++i) {
                    newPots[i - firstFull + 4] = next[i];
                }
                pots = new string(newPots);
                // Keep track of which element the first element of the new array corresponds to
                indexOffset = indexOffset + firstFull - 4;

                Int64 sum = 0;
                for (Int64 i = 0; i < pots.Length; ++i) {
                    if (pots[(int)i] == '#') {
                        sum += (i + indexOffset);
                    }
                }

                Console.WriteLine($"{g,2:D} {indexOffset,3:D}: {pots} sum: {sum}");
                if (gens.ContainsKey(pots)) {
                    Console.WriteLine($"duplicate gen detected: {gens[pots]}");
                    //break;
                } else {
                    gens.Add(pots, g);
                }
            }
        }
    }
}
