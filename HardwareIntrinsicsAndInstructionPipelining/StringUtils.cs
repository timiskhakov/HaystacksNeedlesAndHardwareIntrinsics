using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    public static class StringUtils
    {
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

                for (var i = 0; i < origin.Length; i += Vector128<ushort>.Count)
                {
                    var firstBlock = Sse2.LoadVector128(pOrigin + i);
                    var lastBlock = Sse2.LoadVector128(pOrigin + i + str.Length - 1);

                    var firstMatch = Sse2.CompareEqual(first, firstBlock);
                    var lastMatch = Sse2.CompareEqual(last, lastBlock);

                    var and = Sse2.And(firstMatch, lastMatch);
                    var bytes = and.AsByte();
                    var mask = Sse2.MoveMask(bytes);
                    while (mask > 0)
                    {
                        var positionInBytes = GetFirstSetBitPosition(mask);
                        var position = Math.DivRem(positionInBytes, 2, out var rem);
                        if (rem == 0)
                        {
                            if (IsMatch(pOrigin, i + position - 1, pStr, 0, str.Length))
                            {
                                return i + position - 1;
                            }
                        }

                        mask = SetLowestBitToZero(mask);
                    }
                }
            }

            return -1;
        }

        private static int GetFirstSetBitPosition(int number) => (int)(Math.Log10(number & -number) / Math.Log10(2)) + 1;

        private static int SetLowestBitToZero(int number) => number & (number - 1);

        private static unsafe bool IsMatch(ushort* source, int sourceOffset, ushort* dest, int destOffset, int length)
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