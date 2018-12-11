using System;

namespace day11_2 {
    class Program {
        const Int64 SERIAL_NUMBER = 8444;
        static Int64 CellPower(Int64 x, Int64 y) {
            Int64 rackId = x + 10;
            Int64 power = ((rackId * y) + SERIAL_NUMBER) * rackId;
            return ((power / 100) % 10) - 5;
        }
        static void Main(string[] args) {
            Int64[,] powers = new Int64[300, 300];
            for (Int64 y = 0; y < 300; ++y) {
                for (Int64 x = 0; x < 300; ++x) {
                    powers[y, x] = CellPower(x, y);
                }
            }
            Int64 maxPower = Int64.MinValue, maxX = -1, maxY = -1, maxS = -1;
            Int64[,] sums = new long[300, 300];
            for (Int64 s = 1; s <= 300; ++s) {
                Console.WriteLine($"Testing {s}x{s} squares...");
                for (Int64 y = 0; y < 300 - s + 1; ++y) {
                    for (Int64 x = 0; x < 300 - s + 1; ++x) {
                        Int64 squarePower = 0;
                        if (s == 1) {
                            squarePower = powers[y, x];
                        } else {
                            squarePower = sums[y, x];
                            Int64 rowY = y + s - 1;
                            Int64 colX = x + s - 1;
                            for (Int64 sy = 0; sy < s - 1; ++sy) {
                                squarePower += powers[y + sy, colX];
                            }
                            for (Int64 sx = 0; sx < s; ++sx) {
                                squarePower += powers[rowY, x + sx];
                            }
                        }
                        sums[y, x] = squarePower;
                        if (squarePower > maxPower) {
                            maxPower = squarePower;
                            maxX = x;
                            maxY = y;
                            maxS = s;
                        }
                    }
                }
            }
            Console.WriteLine($"Square at  {maxX},{maxY},{maxS} with grid serial number {SERIAL_NUMBER}: power level {maxPower}");
        }
    }
}
