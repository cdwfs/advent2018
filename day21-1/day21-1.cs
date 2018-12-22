using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day21_1 {
    class Program {
        const int REG_COUNT = 6;
        delegate void ProcessOp(Int64 a, Int64 b, Int64 c, Int64[] regs);
        static Dictionary<string, ProcessOp> ops = new Dictionary<string, ProcessOp> {
            ["addr"] = (a, b, c, regs) => regs[c] = (regs[a] + regs[b]),
            ["addi"] = (a, b, c, regs) => regs[c] = (regs[a] + b),
            ["mulr"] = (a, b, c, regs) => regs[c] = (regs[a] * regs[b]),
            ["muli"] = (a, b, c, regs) => regs[c] = (regs[a] * b),
            ["banr"] = (a, b, c, regs) => regs[c] = (regs[a] & regs[b]),
            ["bani"] = (a, b, c, regs) => regs[c] = (regs[a] & b),
            ["borr"] = (a, b, c, regs) => regs[c] = (regs[a] | regs[b]),
            ["bori"] = (a, b, c, regs) => regs[c] = (regs[a] | b),
            ["setr"] = (a, b, c, regs) => regs[c] = (regs[a]),
            ["seti"] = (a, b, c, regs) => regs[c] = (a),
            ["gtir"] = (a, b, c, regs) => regs[c] = ((a > regs[b]) ? 1 : 0),
            ["gtri"] = (a, b, c, regs) => regs[c] = ((regs[a] > b) ? 1 : 0),
            ["gtrr"] = (a, b, c, regs) => regs[c] = ((regs[a] > regs[b]) ? 1 : 0),
            ["eqir"] = (a, b, c, regs) => regs[c] = ((a == regs[b]) ? 1 : 0),
            ["eqri"] = (a, b, c, regs) => regs[c] = ((regs[a] == b) ? 1 : 0),
            ["eqrr"] = (a, b, c, regs) => regs[c] = ((regs[a] == regs[b]) ? 1 : 0),
        };
        class Instruction {
            public string Opcode;
            public Int64 A, B, C;
        }
        static string Solve(string inputFilename, bool enableLogs) {
            int ipReg = -1;
            var program = new List<Instruction>();
            {
                Regex rx0 = new Regex(@"^#ip\s+(?<reg>\d)", RegexOptions.Compiled);
                Regex rx1 = new Regex(@"^(?<opcode>\w+)\s+(?<a>\d+)\s+(?<b>\d+)\s+(?<c>\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                var m = rx0.Match(inStream.ReadLine());
                ipReg = Int32.Parse(m.Groups["reg"].Value);
                string line;
                while ((line = inStream.ReadLine()) != null) {
                    m = rx1.Match(line);
                    program.Add(new Instruction {
                        Opcode = m.Groups["opcode"].Value,
                        A = Int64.Parse(m.Groups["a"].Value),
                        B = Int64.Parse(m.Groups["b"].Value),
                        C = Int64.Parse(m.Groups["c"].Value),
                    });
                }
                inStream.Dispose();
            }

            Int64[] regs = new Int64[REG_COUNT];
            Int64 instCount = 0;
            regs[0] = 0; // change me
            var results = new List<Int64>();
            var resultsSet = new HashSet<Int64>();
            Int64 resultBeforeLoop = 0;
            for (Int64 ip = 0; ip >= 0 && ip < program.Count; ++ip) {
                var inst = program[(int)ip];
                regs[ipReg] = ip;
                if (ip == 28) {
                    if (!resultsSet.Contains(regs[3])) {
                        resultsSet.Add(regs[3]);
                        results.Add(regs[3]);
                        resultBeforeLoop = regs[3];
                    } else {
                        int prevIndex = results.FindIndex(e => e == regs[3]);
                        Console.WriteLine($"result {regs[3]} previously found at {prevIndex}");
                        Console.WriteLine($"prev result was {resultBeforeLoop}");
                    }
                }
                if (enableLogs) {
                    Console.Write($"{ip,2:D}: [{regs[0],8:D}, {regs[1],8:D}, {regs[2],8:D}, {regs[3],8:D}, {regs[4],8:D}, {regs[5],2:D}] ");
                    Console.Write($"{inst.Opcode} {inst.A,8:D} {inst.B,8:D} {inst.C,1:D}" );
                }
                ops[inst.Opcode](inst.A, inst.B, inst.C, regs);
                if (enableLogs) {
                    Console.WriteLine($"[{regs[0],8:D}, {regs[1],8:D}, {regs[2],8:D}, {regs[3],8:D}, {regs[4],8:D}, {regs[5],2:D}] ");
                }
                ip = regs[ipReg];
                instCount++;
            }
            return instCount.ToString();
        }
        static void Main(string[] args) {
            Console.WriteLine(Solve(args[0] + @"\day21-input.txt", false));
        }
    }
}
