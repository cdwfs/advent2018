using System;

namespace day11_1 {
    class Program {
        const Int64 SERIAL_NUMBER = 8444;
        static Int64 CellPower(Int64 x, Int64 y) {
            Int64 rackId = x + 10;
            Int64 power = ((rackId * y) + SERIAL_NUMBER) * rackId;
            return ((power / 100) % 10) - 5;
        }
        static void Main(string[] args) {
            Int64[,] powers = new Int64[300,300];
            Int64 maxPower = Int64.MinValue, maxX = -1, maxY = -1;
            for (Int64 y = 299; y >= 0; --y) {
                for (Int64 x = 299; x >= 0; --x) {
                    powers[y, x] = CellPower(x, y);
                    if (x + 3 < 300 && y + 3 < 300) {
                        Int64 squarePower =
                            powers[y + 0, x + 0] +
                            powers[y + 0, x + 1] +
                            powers[y + 0, x + 2] +
                            powers[y + 1, x + 0] +
                            powers[y + 1, x + 1] +
                            powers[y + 1, x + 2] +
                            powers[y + 2, x + 0] +
                            powers[y + 2, x + 1] +
                            powers[y + 2, x + 2];
                        if (squarePower > maxPower) {
                            maxPower = squarePower;
                            maxX = x;
                            maxY = y;
                        }
                    }
                }
            }
            Console.WriteLine($"Fuel cell at  {maxX},{maxY}, grid serial number {SERIAL_NUMBER}: power level {maxPower}");
        }
    }
}
