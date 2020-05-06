using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public static class StringUtils
    {
        public static unsafe int IndexOf(string haystack, string needle)
        {
            if (string.IsNullOrEmpty(haystack) || string.IsNullOrEmpty(needle))
            {
                return -1;
            }
            
            var blockSize = Vector256<ushort>.Count;
            
            fixed (char* pHaystackChar = haystack)
            fixed (char* pNeedleChar = needle)
            {
                var pHaystack = (ushort*) pHaystackChar;
                var pNeedle = (ushort*) pNeedleChar;
                
                var needleFirst = Vector256.Create(pNeedle[0]);
                var needleLast = Vector256.Create(pNeedle[needle.Length - 1]);
                for (var i = 0; i < haystack.Length; i += blockSize)
                {
                    var blockFirst = Avx.LoadVector256(pHaystack + i);
                    var blockLast = Avx.LoadVector256(pHaystack + i + needle.Length - 1);

                    var matchFirst = Avx2.CompareEqual(needleFirst, blockFirst);
                    var matchLast = Avx2.CompareEqual(needleLast, blockLast);

                    var and = Avx2.And(matchFirst, matchLast);
                    var maskBytes = Avx2.MoveMask(and.AsByte());
                    var mask = RemoveOddBits(maskBytes);
                    while (mask > 0)
                    {
                        var position = GetFirstBit(mask);
                        if (Compare(pHaystack, i + position + 1, pNeedle, 1, needle.Length - 2))
                        {
                            return i + position;
                        }

                        mask = ClearFirstBit(mask);
                    }
                }
            }

            return -1;
        }

        private static int RemoveOddBits(int n)
        {
            n = ((n & 0x44444444) >> 1) | (n & 0x11111111);
            n = ((n & 0x30303030) >> 2) | (n & 0x03030303);
            n = ((n & 0x0F000F00) >> 4) | (n & 0x000F000F);
            n = ((n & 0x00FF0000) >> 8) | (n & 0x000000FF);
            return n;
        }

        private static int GetFirstBit(int n)
        {
            return (int) (Math.Log10(n & -n) / Math.Log10(2));
        }

        private static int ClearFirstBit(int n)
        {
            return n & (n - 1);
        }

        private static unsafe bool Compare(ushort* source, int sourceOffset, ushort* dest, int destOffset, int length)
        {
            for (var i = 0; i < length; i++)
            {
                if (source[sourceOffset + i] != dest[destOffset + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}