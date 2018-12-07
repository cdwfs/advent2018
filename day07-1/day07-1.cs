using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day07_1 {
    class Program {
        static void Main(string[] args) {
            Regex rx = new Regex(@"^Step (?<pre>\w) .+ step (?<post>\w) can", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var stepPrereqCounts = new int[26];
            var stepDeps = new List<int>[26];
            for (int i = 0; i < stepPrereqCounts.Length; ++i) {
                stepPrereqCounts[i] = -1;
                stepDeps[i] = new List<int>();
            }
            while ((line = inStream.ReadLine()) != null) {
                var m = rx.Match(line);
                int preIdx = Char.Parse(m.Groups["pre"].ToString()) - 'A';
                int postIdx = Char.Parse(m.Groups["post"].ToString()) - 'A';
                stepDeps[preIdx].Add(postIdx);
                stepPrereqCounts[postIdx] = Math.Max(1, stepPrereqCounts[postIdx] + 1);
                stepPrereqCounts[preIdx] = Math.Max(0, stepPrereqCounts[preIdx]);
            }

            bool done = false;
            while (!done) {
                bool foundReadyStep = false;
                for (int i = 0; i < 26; ++i) {
                    if (stepPrereqCounts[i] == 0) {
                        foreach (var dep in stepDeps[i]) {
                            stepPrereqCounts[dep] -= 1;
                            Debug.Assert(stepPrereqCounts[dep] >= 0);
                        }
                        Console.Write($"{(char)('A'+i)}");
                        stepPrereqCounts[i] = -1;
                        foundReadyStep = true;
                        break;
                    }
                }
                if (!foundReadyStep) {
                    done = true;
                    break;
                }
            }
            Console.WriteLine($"\n");
        }
    }
}
