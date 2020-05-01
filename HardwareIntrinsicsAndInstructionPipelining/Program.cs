using BenchmarkDotNet.Running;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Comparison>();
        }
    }
}