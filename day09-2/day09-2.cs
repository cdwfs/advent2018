using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day09_2 {
    class Program {
        class Marble {
            public Marble(UInt64 value) {
                Value = value;
                CW = this;
                CCW = this;
            }
            public Marble(UInt64 value, Marble cw, Marble ccw) {
                Value = value;
                CW = cw;
                CCW = ccw;
                cw.CCW = this;
                ccw.CW = this;
            }
            public UInt64 Value { get; }
            public Marble CW { get; private set; }
            public Marble CCW { get; private set; }

            public (Marble newCurrent, UInt64 pointsEarned) Play(UInt64 value) {
                UInt64 pointsEarned = 0;
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
            var match = rx.Match(line);
            UInt64 playerCount = UInt64.Parse(match.Groups["players"].Value);
            UInt64 lastMarblePoints = 100 * UInt64.Parse(match.Groups["points"].Value);
            Marble currentMarble = new Marble(0);
            UInt64[] playerScores = new UInt64[playerCount];
            UInt64 currentPlayer = 0;
            for (UInt64 m = 1; m <= lastMarblePoints; ++m) {
                (Marble newCurrent, UInt64 pointsEarned) = currentMarble.Play(m);
                playerScores[currentPlayer] += pointsEarned;
                currentMarble = newCurrent;
                currentPlayer = (currentPlayer + 1) % playerCount;
            }
            UInt64 winner = 0;
            UInt64 maxScore = 0;
            for (UInt64 p = 0; p < playerCount; ++p) {
                if (playerScores[p] > maxScore) {
                    maxScore = playerScores[p];
                    winner = p;
                }
            }
            Console.WriteLine($"Winner is player {winner} (zero-based) with a score of {maxScore}");
        }
    }
}
