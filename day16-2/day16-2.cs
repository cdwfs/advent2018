using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day16_2 {
    class Program {
        enum Arg {
            VAL = 0,
            REG = 1,
        }
        enum OpcodeId {
            ADDR =  0,
            ADDI =  1,
            MULR =  2,
            MULI =  3,
            BANR =  4,
            BANI =  5,
            BORR =  6,
            BORI =  7,
            SETR =  8,
            SETI =  9,
            GTIR = 10,
            GTRI = 11,
            GTRR = 12,
            EQIR = 13,
            EQRI = 14,
            EQRR = 15,
        }
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
            public bool Validate(Arg aType, Arg bType, Arg cType) {
                // Make sure the instruction itself is well-formed
                if (Inst.Op < 0 || Inst.Op > 15) {
                    return false; // invalid opcode
                }
                // Make sure the only register that changed is the output (C)
                Debug.Assert(IsCReg);
                for (int r = 0; r < 4; ++r) {
                    if (r == Inst.C)
                        continue;
                    if (Before[r] != After[r]) {
                        return false;
                    }
                }
                // range-check any arg that's supposed to be a register
                if (aType == Arg.REG && !IsAReg) {
                    return false; // register A out of range
                }
                if (bType == Arg.REG && !IsBReg) {
                    return false; // register B out of range
                }
                if (cType == Arg.REG && !IsCReg) {
                    return false; // register C out of range
                }
                // all good!
                return true;
            }
        }
        static void ConstrainOp(Operation op, int[] opMasks) {
            // addr(add register) stores into register C the result of adding register A and register B.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] + op.Before[op.Inst.B])) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.ADDR);
            }
            // addi(add immediate) stores into register C the result of adding register A and value B.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] + op.Inst.B)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.ADDI);
            }
            // mulr(multiply register) stores into register C the result of multiplying register A and register B.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] * op.Before[op.Inst.B])) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.MULR);
            }
            // muli(multiply immediate) stores into register C the result of multiplying register A and value B.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] * op.Inst.B)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.MULI);
            }
            // banr(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] & op.Before[op.Inst.B])) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.BANR);
            }
            // bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] & op.Inst.B)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.BANI);
            }
            // borr(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] | op.Before[op.Inst.B])) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.BORR);
            }
            // bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] | op.Inst.B)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.BORI);
            }
            // setr(set register) copies the contents of register A into register C. (Input B is ignored.)
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A])) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.SETR);
            }
            // seti(set immediate) stores value A into register C. (Input B is ignored.)
            if (op.Validate(Arg.VAL, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Inst.A)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.SETI);
            }
            // gtir(greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.VAL, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Inst.A > op.Before[op.Inst.B] ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.GTIR);
            }
            // gtri(greater - than register / immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] > op.Inst.B ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.GTRI);
            }
            // gtrr(greater - than register / register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] > op.Before[op.Inst.B] ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.GTRR);
            }
            // eqir(equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.VAL, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Inst.A == op.Before[op.Inst.B] ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.EQIR);
            }
            // eqri(equal register / immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.REG, Arg.VAL, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] == op.Inst.B ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.EQRI);
            }
            // eqrr(equal register / register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
            if (op.Validate(Arg.REG, Arg.REG, Arg.REG) &&
                op.After[op.Inst.C] != (op.Before[op.Inst.A] == op.Before[op.Inst.B] ? 1 : 0)) {
                opMasks[op.Inst.Op] &= ~(1 << (int)OpcodeId.EQRR);
            }
        }
        static void ExecuteInstruction(Instruction inst, OpcodeId[] opcodeMap, int[] regs) {
            if (opcodeMap[inst.Op] == OpcodeId.ADDR) {
                // addr(add register) stores into register C the result of adding register A and register B.
                regs[inst.C] = regs[inst.A] + regs[inst.B];
            } else if (opcodeMap[inst.Op] == OpcodeId.ADDI) {
                // addi(add immediate) stores into register C the result of adding register A and value B.
                regs[inst.C] = regs[inst.A] + inst.B;
            } else if (opcodeMap[inst.Op] == OpcodeId.MULR) {
                // mulr(multiply register) stores into register C the result of multiplying register A and register B.
                regs[inst.C] = regs[inst.A] * regs[inst.B];
            } else if (opcodeMap[inst.Op] == OpcodeId.MULI) {
                // muli(multiply immediate) stores into register C the result of multiplying register A and value B.
                regs[inst.C] = regs[inst.A] * inst.B;
            } else if (opcodeMap[inst.Op] == OpcodeId.BANR) {
                // banr(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
                regs[inst.C] = regs[inst.A] & regs[inst.B];
            } else if (opcodeMap[inst.Op] == OpcodeId.BANI) {
                // bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
                regs[inst.C] = regs[inst.A] & inst.B;
            } else if (opcodeMap[inst.Op] == OpcodeId.BORR) {
                // borr(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
                regs[inst.C] = regs[inst.A] | regs[inst.B];
            } else if (opcodeMap[inst.Op] == OpcodeId.BORI) {
                // bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
                regs[inst.C] = regs[inst.A] | inst.B;
            } else if (opcodeMap[inst.Op] == OpcodeId.SETR) {
                // setr(set register) copies the contents of register A into register C. (Input B is ignored.)
                regs[inst.C] = regs[inst.A];
            } else if (opcodeMap[inst.Op] == OpcodeId.SETI) {
                // seti(set immediate) stores value A into register C. (Input B is ignored.)
                regs[inst.C] = inst.A;
            } else if (opcodeMap[inst.Op] == OpcodeId.GTIR) {
                // gtir(greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
                regs[inst.C] = (inst.A > regs[inst.B]) ? 1 : 0;
            } else if (opcodeMap[inst.Op] == OpcodeId.GTRI) {
                // gtri(greater - than register / immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
                regs[inst.C] = (regs[inst.A] > inst.B) ? 1 : 0;
            } else if (opcodeMap[inst.Op] == OpcodeId.GTRR) {
                // gtrr(greater - than register / register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
                regs[inst.C] = (regs[inst.A] > regs[inst.B]) ? 1 : 0;
            } else if (opcodeMap[inst.Op] == OpcodeId.EQIR) {
                // eqir(equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
                regs[inst.C] = (inst.A == regs[inst.B]) ? 1 : 0;
            } else if (opcodeMap[inst.Op] == OpcodeId.EQRI) {
                // eqri(equal register / immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
                regs[inst.C] = (regs[inst.A] == inst.B) ? 1 : 0;
            } else if (opcodeMap[inst.Op] == OpcodeId.EQRR) {
                // eqrr(equal register / register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
                regs[inst.C] = (regs[inst.A] == regs[inst.B]) ? 1 : 0;
            }
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
                        Inst = new Instruction { Op = op, A = argA, B = argB, C = argC },
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

            int[] opMasks = new int[16];
            for (int i = 0; i < opMasks.Length; ++i) {
                opMasks[i] = 0xFFFF;
            }
            foreach (var op in operations) {
                ConstrainOp(op, opMasks);
            }
            for (var i = 0; i < opMasks.Length; ++i) {
                //Console.WriteLine($"opcode {i,2:D}: {Convert.ToString(opMasks[i], 2).PadLeft(16, '0')}");
            }

            OpcodeId[] opcodeMap = new OpcodeId[16];
            for (int iteration = 0; iteration < 16; ++iteration) {
                for (int id = 0; id < 16; ++id) {
                    int matches = 0;
                    int which = -1;
                    for (int opcode = 0; opcode < 16; ++opcode) {
                        if ((opMasks[opcode] & (1 << id)) != 0) {
                            matches += 1;
                            which = opcode;
                        }
                    }
                    if (matches == 1) {
                        //Console.WriteLine($"{iteration}: opcode {which} must be operation {id}={((OpcodeId)id).ToString()}");
                        opcodeMap[which] = (OpcodeId)id;
                        opMasks[which] = (1 << id);
                    }
                }
            }

            int[] regs = new int[4];
            foreach (var inst in testProgram) {
                ExecuteInstruction(inst, opcodeMap, regs);
            }

            return regs[0].ToString();
        }
        static void Main(string[] args) {
            Console.WriteLine(Solve(args[0]));
        }
    }
}
