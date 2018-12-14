using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day14_2 {
    class Program {
        static Int64 FoundPattern(int[] scores, int scoreCount) {
            if (scoreCount < 6) {
                return -1;
            }
            if (scores[scoreCount - 1] == 1 &&
                scores[scoreCount - 2] == 4 &&
                scores[scoreCount - 3] == 6 &&
                scores[scoreCount - 4] == 0 &&
                scores[scoreCount - 5] == 7 &&
                scores[scoreCount - 6] == 1) {
                return scoreCount-6;
            }
            return -1;
        }
        static void Main(string[] args) {
            int[] scores = new int[100000000];
            scores[0] = 3;
            scores[1] = 7;
            int scoreCount = 2;
            int currentA = 0;
            int currentB = 1;
            while (scoreCount < scores.Length-1) {
                int sum = scores[currentA] + scores[currentB];
                if (sum >= 10) {
                    scores[scoreCount++] = 1;
                    Int64 start = FoundPattern(scores, scoreCount);
                    if (start >= 0) {
                        Console.WriteLine($"pattern found starting at {start}");
                        return;
                    }
                    scores[scoreCount++] = sum - 10;
                    start = FoundPattern(scores, scoreCount);
                    if (start >= 0) {
                        Console.WriteLine($"pattern found starting at {start}");
                        return;
                    }
                } else {
                    scores[scoreCount++] = sum;
                    Int64 start = FoundPattern(scores, scoreCount);
                    if (start >= 0) {
                        Console.WriteLine($"pattern found starting at {start}");
                        return;
                    }
                }
                currentA = (currentA + 1 + scores[currentA]) % scoreCount;
                currentB = (currentB + 1 + scores[currentB]) % scoreCount;
            }
            Console.WriteLine($"Pattern not count earlier than {scoreCount} :(");
        }
    }
}
