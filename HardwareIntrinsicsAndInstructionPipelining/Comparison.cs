using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    public class Comparison
    {
        private readonly Random _random = new Random();
        private double[] _input;
        
        [Params(1_000, 10_000, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _input = Enumerable.Range(0, N)
                .Select(_ => _random.NextDouble())
                .ToArray();
        }
        
        [Benchmark]
        public byte[] BlockCopy()
        {
            return ArrayConverter.GetBytes(_input);
        }

        [Benchmark]
        public byte[] Vectors()
        {
            return ArrayConverter.GetBytesVectors(_input);
        }
    }
}