using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public class Comparison
    {
        private string _haystack;
        private static readonly string Needle = Guid.NewGuid().ToString();

        [Params(1_000, 10_000, 100_000, 1_000_000, 10_000_000)]
        public int N;
        
        [GlobalSetup]
        public void Setup()
        {
            _haystack = new StringBuilder()
                .Append(string.Join("", Enumerable.Range(0, N).Select(_ => Guid.NewGuid().ToString())))
                .Append(Needle)
                .ToString();
        }
        
        [Benchmark]
        public int IndexOf()
        {
            return _haystack.IndexOf(Needle, StringComparison.Ordinal);
        }

        [Benchmark]
        public int RegEx()
        {
            var match = Regex.Match(_haystack, Needle);
            return match.Success ? match.Index : -1;
        }
        
        [Benchmark]
        public int Intrinsics()
        {
            return StringUtils.IndexOf(_haystack, Needle);
        }
    }
}