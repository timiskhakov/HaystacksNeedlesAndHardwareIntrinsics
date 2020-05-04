using BenchmarkDotNet.Running;
using HardwareIntrinsicsAndInstructionPipelining.Comparisons;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    internal static class Program
    {
        private static void Main()
        {
            //BenchmarkRunner.Run<ConversionComparison>();
            BenchmarkRunner.Run<StringSearchComparison>();
        }
    }
}