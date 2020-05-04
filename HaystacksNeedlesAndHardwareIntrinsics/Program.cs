using BenchmarkDotNet.Running;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Comparison>();
        }
    }
}