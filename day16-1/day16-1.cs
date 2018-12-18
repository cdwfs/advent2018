using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day16_1 {
    class Program {
        struct Instruction {
            public int Op;
            public int A, B, C;
            public bool IsAReg { get => A < 4; }
            public bool IsBReg { get => B < 4; }
            public bool IsCReg { get => C < 4; }
        }
        struct Operation {
            public int[] Before;
            public Instruction Inst;
            public int[] After;
            public bool IsAReg { get => Inst.IsAReg; }
            public bool IsBReg { get => Inst.IsBReg; }
            public bool IsCReg { get => Inst.IsCReg; }
        }
        static int CountOpMatches(Operation op) {
            int matches = 0;
            // addr(add register) stores into register C the result of adding register A and register B.
            if (op.IsAReg && op.IsBReg && op.IsCReg &&
                op.After[op.Inst.C] == op.Before[op.Inst.A] + op.Before[op.Inst.B]) {
            }
            // addi(add immediate) stores into register C the result of adding register A and value B.
            // mulr(multiply register) stores into register C the result of multiplying register A and register B.
            // muli(multiply immediate) stores into register C the result of multiplying register A and value B.
            // banr(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
            // bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
            // borr(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
            // bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
            // setr(set register) copies the contents of register A into register C. (Input B is ignored.)
            // seti(set immediate) stores value A into register C. (Input B is ignored.)
            // gtir(greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
            // gtri(greater - than register / immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
            // gtrr(greater - than register / register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
            // eqir(equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
            // eqri(equal register / immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
            // eqrr(equal register / register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
            return matches;
        }
        static string Solve(string inputFile) {
            List<Operation> operations = new List<Operation>();
            List<Instruction> testProgram = new List<Instruction>();
            {
                Regex rxBefore = new Regex(@"^Before:\s+\[(?<R0>\d+),\s+(?<R1>\d+),\s+(?<R2>\d+),\s+(?<R3>\d+)\]", RegexOptions.Compiled);
                Regex rxInst = new Regex(@"^(?<Op>\d+)\s+(?<A>\d+)\s+(?<B>\d+)\s+(?<C>\d+)", RegexOptions.Compiled);
                Regex rxAfter = new Regex(@"^After:\s+\[(?<R0>\d+),\s+(?<R1>\d+),\s+(?<R2>\d+),\s+(?<R3>\d+)\]", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFile);
                string line;
                bool readingTestProgram = false;
                // read operations
                while (!readingTestProgram) {
                    line = inStream.ReadLine();
                    var m = rxBefore.Match(line);
                    if (!m.Success) {
                        Debug.Assert(line.Length == 0, "expected blank line");
                        readingTestProgram = true;
                        break;
                    }

                    int b0 = Int32.Parse(m.Groups["R0"].Value);
                    int b1 = Int32.Parse(m.Groups["R1"].Value);
                    int b2 = Int32.Parse(m.Groups["R2"].Value);
                    int b3 = Int32.Parse(m.Groups["R3"].Value);
                    line = inStream.ReadLine();
                    m = rxInst.Match(line);
                    int op = Int32.Parse(m.Groups["Op"].Value);
                    int argA = Int32.Parse(m.Groups["A"].Value);
                    int argB = Int32.Parse(m.Groups["B"].Value);
                    int argC = Int32.Parse(m.Groups["C"].Value);
                    line = inStream.ReadLine();
                    m = rxAfter.Match(line);
                    int a0 = Int32.Parse(m.Groups["R0"].Value);
                    int a1 = Int32.Parse(m.Groups["R1"].Value);
                    int a2 = Int32.Parse(m.Groups["R2"].Value);
                    int a3 = Int32.Parse(m.Groups["R3"].Value);
                    line = inStream.ReadLine(); // blank line
                    Debug.Assert(line.Length == 0);

                    operations.Add(new Operation {
                        Before = new int[4] { b0, b1, b2, b3 },
                        Instruction = new Instruction { Op = op, A = argA, B = argB, C = argC },
                        After = new int[4] { a0, a1, a2, a3 },
                    });
                }
                // Read test program
                line = inStream.ReadLine();
                Debug.Assert(line.Length == 0, "expected blank line");
                while ((line = inStream.ReadLine()) != null) {
                    var m = rxInst.Match(line);
                    int op = Int32.Parse(m.Groups["Op"].Value);
                    int argA = Int32.Parse(m.Groups["A"].Value);
                    int argB = Int32.Parse(m.Groups["B"].Value);
                    int argC = Int32.Parse(m.Groups["C"].Value);
                    testProgram.Add(new Instruction {
                        Op = op,
                        A = argA,
                        B = argB,
                        C = argC,
                    });
                }
                inStream.Dispose();
            }


            return "TBD";
        }
        static void Main(string[] args) {
            Console.WriteLine(Solve(args[0]));
        }
    }
}
