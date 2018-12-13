using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day04_1 {
    struct GuardStats {
        public int id;
        public int totalMinutesAsleep;
        public int[] asleepMinuteCounts;
    }
    class Program {
        static void Main(string[] args) {
            Regex rx = new Regex(@"^\[\d{4}\-\d{2}\-\d{2} \d{2}\:(?<minute>\d{2})\] (?<word1>\w+) #?(?<word2>\w+)\s*", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var guardStats = new Dictionary<int, GuardStats>();
            int currentGuard = -1;
            int sleepStart = -1;
            while ((line = inStream.ReadLine()) != null) {
                var m = rx.Match(line);
                Debug.Assert(m.Success, "regex did not match line!");
                int minute = Int32.Parse(m.Groups["minute"].Value);
                string w1 = m.Groups["word1"].Value;
                if (w1 == "Guard") {
                    // if current guard was asleep, mark the rest of the hour as asleep
                    Debug.Assert(sleepStart == -1, "shift change while asleep");
                    // new guard on duty
                    currentGuard = Int32.Parse(m.Groups["word2"].Value);
                    if (!guardStats.ContainsKey(currentGuard)) {
                        var newGuard = new GuardStats {
                            id = currentGuard,
                            totalMinutesAsleep = 0,
                            asleepMinuteCounts = new int[60]
                        };
                        guardStats.Add(currentGuard, newGuard);
                    }
                } else if (w1 == "falls") {
                    Debug.Assert(sleepStart == -1, "asleep guard fell asleep");
                    sleepStart = minute;
                } else if (w1 == "wakes") {
                    Debug.Assert(sleepStart >= 0, "awake guard woke up");
                    var guard = guardStats[currentGuard];
                    guard.totalMinutesAsleep += (minute - sleepStart);
                    // TODO: mark everything between sleepStart and minute in guard stats
                    for (int i = sleepStart; i < minute; ++i) {
                        guard.asleepMinuteCounts[i] += 1;
                    }
                    guardStats[currentGuard] = guard;
                    sleepStart = -1;
                }

                //string w2 = m.Groups["word2"].Value;
                //Console.WriteLine($"{minute} {w1} {w2}");
            }

            var allGuards = guardStats.Values;
            int sleepiestGuard = -1;
            int mostAsleepMinute = -1;
            int maxSleepCount = -1;
            foreach (var g in allGuards) {
                for (int i = 0; i < 60; ++i) {
                    if (g.asleepMinuteCounts[i] > maxSleepCount) {
                        sleepiestGuard = g.id;
                        mostAsleepMinute = i;
                        maxSleepCount = g.asleepMinuteCounts[i];
                    }
                }
            }
            Console.WriteLine($"guard {sleepiestGuard} was asleep {maxSleepCount} times at minute {mostAsleepMinute}");
            Console.WriteLine($"so that's {sleepiestGuard * mostAsleepMinute}");

        }
    }
}
