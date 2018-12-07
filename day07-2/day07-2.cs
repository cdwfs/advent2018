using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day07_2 {
    class Program {
        const int WORKER_COUNT = 5;
        const int DURATION_BASE = 61;
        static void Main(string[] args) {
            Regex rx = new Regex(@"^Step (?<pre>\w) .+ step (?<post>\w) can", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var stepPrereqCounts = new int[26];
            var stepDeps = new List<int>[26];
            var stepIsClaimed = new bool[26];
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
            int stepCount = 0;
            foreach (var count in stepPrereqCounts) {
                if (count >= 0) {
                    stepCount += 1;
                }
            }

            var workers = new (int step, int duration)[WORKER_COUNT];
            int ticks = 0;
            while (stepCount > 0) {
                for (int w = 0; w < workers.Length; ++w) {
                    if (workers[w].duration > 1) {
                        Debug.Assert(workers[w].step != -1);
                        workers[w].duration -= 1;
                    } else if (workers[w].duration == 1) {
                        int step = workers[w].step;
                        Debug.Assert(step != -1);
                        Console.WriteLine($"{ticks,3:D}: worker {w} finished step {(char)('A' + step)}");
                        foreach (var dep in stepDeps[step]) {
                            stepPrereqCounts[dep] -= 1;
                            Debug.Assert(stepPrereqCounts[dep] >= 0);
                        }
                        stepPrereqCounts[step] = -1;
                        stepCount -= 1;
                        workers[w].duration = 0;
                        workers[w].step = -1;
                    }
                }
                for (int w = 0; w < workers.Length; ++w) {
                    if (workers[w].duration == 0) {
                        // look for new step for worker
                        for (int i = 0; i < 26; ++i) {
                            if (stepPrereqCounts[i] == 0 && !stepIsClaimed[i]) {
                                workers[w].step = i;
                                workers[w].duration = i + DURATION_BASE;
                                stepIsClaimed[i] = true;
                                Console.WriteLine($"{ticks,3:D}: worker {w} started step {(char)('A' + i)} with duration {i + DURATION_BASE}");
                                break;
                            }
                        }
                    }
                }
                ticks += 1;
            }
            Console.WriteLine($"{ticks-1}\n");
        }
    }
}
