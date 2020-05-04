using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace HaystacksNeedlesAndHardwareIntrinsics
{
    public static class StringUtils
    {
        public static unsafe int IndexOf(string origin, string str)
        {
            fixed (char* pOrigin = origin)
            fixed (char* pStr = str)
            {
                var first = Vector128.Create(pStr[0]);
                var last = Vector128.Create(pStr[str.Length - 1]);
                for (var i = 0; i < origin.Length; i += Vector128<ushort>.Count)
                {
                    var firstBlock = Sse2.LoadVector128((ushort*) pOrigin + i);
                    var lastBlock = Sse2.LoadVector128((ushort*) pOrigin + i + str.Length - 1);

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

        private static unsafe bool Compare(char* source, int sourceOffset, char* dest, int destOffset, int length)
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