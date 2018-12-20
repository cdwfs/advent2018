using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day19_1 {
    class Program {
        const int REG_COUNT = 6;
        delegate void ProcessOp(int a, int b, int c, int[] regs);
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
            public int A, B, C;
        }
        static string Solve(string inputFilename, bool enableLogs) {
            int ipReg = -1;
            Instruction[] program;
            {
                Regex rx0 = new Regex(@"^#ip\s+(?<reg>\d)", RegexOptions.Compiled);
                Regex rx1 = new Regex(@"^(?<opcode>\w+)\s+(?<a>\d+)\s+(?<b>\d+)\s+(?<c>\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                var m = rx0.Match(inStream.ReadLine());
                ipReg = Int32.Parse(m.Groups["reg"].Value);
                string line;
                var instList = new List<Instruction>();
                while ((line = inStream.ReadLine()) != null) {
                    m = rx1.Match(line);
                    instList.Add(new Instruction {
                        Opcode = m.Groups["opcode"].Value,
                        A = Int32.Parse(m.Groups["a"].Value),
                        B = Int32.Parse(m.Groups["b"].Value),
                        C = Int32.Parse(m.Groups["c"].Value),
                    });
                }
                inStream.Dispose();
                program = instList.ToArray();
            }

            int[] regs = new int[REG_COUNT];
            for (int ip = 0; ip >= 0 && ip < program.Length; ++ip) {
                var inst = program[ip];
                regs[ipReg] = ip;
                ops[inst.Opcode](inst.A, inst.B, inst.C, regs);
                ip = regs[ipReg];
            }
            return regs[0].ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day19-example0.txt", true) == "6");
            Console.WriteLine(Solve(args[0] + @"\day19-input.txt", false));
        }
    }
}
