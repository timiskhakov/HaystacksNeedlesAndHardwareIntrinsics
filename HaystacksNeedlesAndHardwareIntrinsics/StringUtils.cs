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
                    
                    var maskBytes = Sse2.MoveMask(Sse2.And(firstMatch, lastMatch).AsByte());
                    var mask = RemoveOddBits(maskBytes);
                    while (mask > 0)
                    {
                        var position = GetFirstBit(mask);
                        if (Compare(pOrigin, i + position, pStr, 1, str.Length - 2))
                        {
                            return i + position - 1;
                        }

                        mask = ClearFirstBit(mask);
                    }
                }
            }

            return -1;
        }

        private static int RemoveOddBits(int number)
        {
            number = ((number & 0x44444444) >> 1) | (number & 0x11111111);
            number = ((number & 0x30303030) >> 2) | (number & 0x03030303);
            number = ((number & 0x0F000F00) >> 4) | (number & 0x000F000F);
            number = ((number & 0x00FF0000) >> 8) | (number & 0x000000FF);
            return number;
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