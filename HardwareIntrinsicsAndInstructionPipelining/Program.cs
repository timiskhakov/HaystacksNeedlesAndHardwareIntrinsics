using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    internal static class Program
    {
        private static void Main()
        {
            var random = new Random();
            var doubles = Enumerable.Range(0, 100)
                .Select(_ => random.NextDouble() * random.Next(-100, 100))
                .ToArray();

            var bytes = new byte[doubles.Length * sizeof(double)];
            Buffer.BlockCopy(doubles, 0, bytes, 0, bytes.Length);
            
            var sb1 = new StringBuilder();
            for (var i = 0; i < doubles.Length; i++)
            {
                sb1.Append($"{doubles[i].ToString(CultureInfo.InvariantCulture)}, ");
                if ((i + 1) % 4 == 0)
                {
                    sb1.AppendLine();
                }
            }
            Console.WriteLine(sb1.ToString());
            
            Console.WriteLine();
            Console.WriteLine();
            
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append($"{bytes[i].ToString()}, ");
                if ((i + 1) % 8 == 0)
                {
                    sb.AppendLine();
                }
            }
            Console.WriteLine(sb.ToString());
        }
    }
}