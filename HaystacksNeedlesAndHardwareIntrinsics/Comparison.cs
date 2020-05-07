using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public class Comparison
    {
        private const int S  =     1_000;
        private const int M  =    10_000;
        private const int L  =   100_000;
        private const int XL = 1_000_000;
        
        private static readonly Random Random = new Random();
        private static readonly char[] AllowedChars =
            " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        
        private static readonly Dictionary<int, (string, string)> Data = new Dictionary<int, (string, string)>
        {
            {S, (GenerateRandomHaystack(S), GenerateRandomNeedle(S))},
            {M, (GenerateRandomHaystack(M), GenerateRandomNeedle(M))},
            {L, (GenerateRandomHaystack(L), GenerateRandomNeedle(L))},
            {XL, (GenerateRandomHaystack(XL), GenerateRandomNeedle(XL))}
        };

        private string _haystack;
        private string _needle;

        [Params(S, M, L, XL)]
        public int N;
        
        [GlobalSetup]
        public void Setup()
        {
            (_haystack, _needle) = Data[N];
        }
        
        [Benchmark]
        public int IndexOf()
        {
            return _haystack.IndexOf(_needle, StringComparison.Ordinal);
        }

        [Benchmark]
        public int RegEx()
        {
            var match = Regex.Match(_haystack, _needle, RegexOptions.Compiled);
            return match.Success ? match.Index : -1;
        }
        
        [Benchmark]
        public int Intrinsics()
        {
            return StringUtils.IndexOf(_haystack, _needle);
        }

        private static string GenerateRandomHaystack(int length)
        {
            return new string(Enumerable.Repeat(AllowedChars, length)
                .Select(x => x[Random.Next(x.Length)])
                .ToArray());
        }
        
        private static string GenerateRandomNeedle(int maxLength)
        {
            return new string(Enumerable.Repeat(AllowedChars, Random.Next(maxLength))
                .Select(x => x[Random.Next(x.Length)])
                .ToArray());
        }
    }
}