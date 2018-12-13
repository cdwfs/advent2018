using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day12_1 {
    class Program {
        static void Main(string[] args) {
            const int MAX_GENERATION = 20;
            const int POT_OFFSET = 4 * MAX_GENERATION;
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            char[] pots;
            {
                Regex rx0 = new Regex(@"^initial state: (?<state>[#.]+)", RegexOptions.Compiled);
                line = inStream.ReadLine();
                var match = rx0.Match(line);
                string state = match.Groups["state"].Value;
                int potCount = state.Length + 2 * POT_OFFSET;
                pots = new char[potCount];
                for (int i = 0; i < potCount; ++i) {
                    pots[i] = (i >= POT_OFFSET && i<POT_OFFSET+state.Length) ? state[i - POT_OFFSET] : '.';
                }
                line = inStream.ReadLine(); // blank line
                Console.WriteLine($"{0,2:D}: {new string(pots)}");
            }

            Regex rx = new Regex(@"^(?<pattern>[#.]{5}) => (?<result>[#.])", RegexOptions.Compiled);
            var rules = new Dictionary<string, char>();
            while ((line = inStream.ReadLine()) != null) {
                var match = rx.Match(line);
                string pattern = match.Groups["pattern"].Value;
                char result = Char.Parse(match.Groups["result"].Value);
                rules.Add(pattern, result);
            }

            for (int g = 1; g <= MAX_GENERATION; ++g) {
                char[] newPots = new char[pots.Length];
                newPots[0] = newPots[1] = newPots[pots.Length - 2] = newPots[pots.Length - 1] = '.';
                for (int i = 0; i < pots.Length - 4; ++i) {
                    var seg = new ArraySegment<char>(pots, i, 5);
                    var key = new string(seg.ToArray());
                    newPots[i+2] = rules[key];
                }
                pots = newPots;
                Console.WriteLine($"{g,2:D}: {new string(pots)}");
            }

            int sum = 0;
            for (int i = 0; i < pots.Length; ++i) {
                if (pots[i] == '#') {
                    sum += i - POT_OFFSET;
                }
            }
            Console.WriteLine($"sum is {sum}");
        }
    }
}
