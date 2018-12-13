using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day09_1 {
    class Program {
        class Marble {
            public Marble(int value) {
                Value = value;
                CW = this;
                CCW = this;
            }
            public Marble(int value, Marble cw, Marble ccw) {
                Value = value;
                CW = cw;
                CCW = ccw;
                cw.CCW = this;
                ccw.CW = this;
            }
            public int Value { get; }
            public Marble CW { get; private set;  }
            public Marble CCW { get; private set; }

            public (Marble newCurrent, int pointsEarned) Play(int value) {
                int pointsEarned = 0;
                if ((value % 23) != 0) {
                    Marble mCW1 = CW;
                    Marble mCW2 = mCW1.CW;
                    Marble mNew = new Marble(value, mCW2, mCW1);
                    return (mNew, pointsEarned);
                } else {
                    pointsEarned = value;
                    Marble mCCW6 = CCW.CCW.CCW.CCW.CCW.CCW;
                    Marble mCCW7 = mCCW6.CCW;
                    Marble mCCW8 = mCCW7.CCW;
                    pointsEarned += mCCW7.Value;
                    mCCW6.CCW = mCCW8;
                    mCCW8.CW = mCCW6;
                    return (mCCW6, pointsEarned);
                }
            }
        }
        static void Main(string[] args) {
            Regex rx = new Regex(@"^(?<players>\d+) players; last marble is worth (?<points>\d+) points", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line = inStream.ReadLine();
            inStream.Dispose();
            var match = rx.Match(line);
            int playerCount = Int32.Parse(match.Groups["players"].Value);
            int lastMarblePoints = Int32.Parse(match.Groups["points"].Value);
            Marble currentMarble = new Marble(0);
            int[] playerScores = new int[playerCount];
            int currentPlayer = 0;
            for (int m = 1; m <= lastMarblePoints; ++m) {
                (Marble newCurrent, int pointsEarned) = currentMarble.Play(m);
                playerScores[currentPlayer] += pointsEarned;
                currentMarble = newCurrent;
                currentPlayer = (currentPlayer + 1) % playerCount;
            }
            int winner = 0;
            int maxScore = 0;
            for (int p = 0; p < playerCount; ++p) {
                if (playerScores[p] > maxScore) {
                    maxScore = playerScores[p];
                    winner = p;
                }
            }
            Console.WriteLine($"Winner is player {winner} (zero-based) with a score of {maxScore}");
        }
    }
}
