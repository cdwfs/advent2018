using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day22_1 {
    class Program {
        struct Region {
            public Int64 GeologicIndex;
            public Int64 ErosionLevel;
            public Int64 RiskLevel;
            public char Type;
        };
        const char ROCKY = '.';
        const char WET = '=';
        const char NARROW = '|';
        static string Solve(int depth, (int x,int y) targetPos, bool enableLogs) {
            var map = new Region[targetPos.y + 1, targetPos.x + 1];
            Int64 totalRiskLevel = 0;
            for (int y = 0; y <= targetPos.y; ++y) {
                for (int x = 0; x <= targetPos.x; ++x) {
                    Int64 geologicIndex;
                    if ((x, y) == (0, 0)) { // TIL: tuple equality requires C# 7.3
                        geologicIndex = 0;
                    } else if ((x, y) == targetPos) {
                        geologicIndex = 0;
                    } else if (y == 0) {
                        geologicIndex = x * 16807;
                    } else if (x == 0) {
                        geologicIndex = y * 48271;
                    } else {
                        geologicIndex = map[y, x - 1].ErosionLevel * map[y - 1, x].ErosionLevel;
                    }
                    Int64 erosionLevel = (geologicIndex + depth) % 20183;
                    Int64 riskLevel = erosionLevel % 3;
                    char type;
                    if (riskLevel == 0) {
                        type = ROCKY;
                    } else if (riskLevel == 1) {
                        type = WET;
                    } else {
                        type = NARROW;
                    }
                    map[y, x] = new Region {
                        GeologicIndex = geologicIndex,
                        ErosionLevel = erosionLevel,
                        RiskLevel = riskLevel,
                        Type = type,
                    };
                    totalRiskLevel += riskLevel;
                }
            }
            return totalRiskLevel.ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(510, (10, 10), true) == "114");
            Console.WriteLine(Solve(4002, (5,746), false));
        }
    }
}
