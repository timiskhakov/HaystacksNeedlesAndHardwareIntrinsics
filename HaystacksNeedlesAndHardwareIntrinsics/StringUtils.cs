using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public static class StringUtils
    {
        private static readonly int BatchSize = Vector128<ushort>.Count;

        public static unsafe int IndexOf(string origin, string str)
        {
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(str))
            {
                return -1;
            }
            
            fixed (char* pOriginChar = origin)
            fixed (char* pStrChar = str)
            {
                var pOrigin = (ushort*) pOriginChar;
                var pStr = (ushort*) pStrChar;
                
                var first = Vector128.Create(pStr[0]);
                var last = Vector128.Create(pStr[str.Length - 1]);
                for (var i = 0; i < origin.Length; i += BatchSize)
                {
                    var firstBlock = Sse2.LoadVector128(pOrigin + i);
                    var lastBlock = Sse2.LoadVector128(pOrigin + i + str.Length - 1);

                    var firstMatch = Sse2.CompareEqual(first, firstBlock);
                    var lastMatch = Sse2.CompareEqual(last, lastBlock);
                    
                    var mask = Sse2.MoveMask(Sse2.And(firstMatch, lastMatch).AsByte());
                    while (mask > 0)
                    {
                        var positionInBytes = GetFirstBit(mask);
                        var position = Math.DivRem(positionInBytes, 2, out var rem);
                        if (rem == 0 && Compare(pOrigin, i + position - 1, pStr, 0, str.Length))
                        {
                            return i + position - 1;
                        }

                        mask = ClearFirstBit(mask);
                    }
                }
            }

            return -1;
        }

        private static int GetFirstBit(int number)
        {
            return (int) (Math.Log10(number & -number) / Math.Log10(2)) + 1;
        }

        private static int ClearFirstBit(int number)
        {
            return number & (number - 1);
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