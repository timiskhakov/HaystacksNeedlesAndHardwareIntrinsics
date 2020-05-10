using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public class Comparison
    {
        private string _haystack;
        private string _needle;

        [GlobalSetup]
        public async Task Setup()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "text.txt");
            _haystack = await File.ReadAllTextAsync(file);
            _needle = "LastWord";
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
    }
}