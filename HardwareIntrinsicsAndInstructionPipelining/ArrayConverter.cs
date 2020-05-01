using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace HardwareIntrinsicsAndInstructionPipelining
{
    public static class ArrayConverter
    {
        public static byte[] GetBytes(double[] values)
        {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        
        public static unsafe byte[] GetBytesVectors(double[] values)
        {
            if (!Avx.IsSupported) throw new Exception("AVX is not supported");
            
            var blockSize = Vector256<double>.Count;
            if (values.Length < blockSize) throw new Exception("Give me more doubles!");

            var result = new byte[values.Length * sizeof(double)];

            fixed (double* pValues = values)
            fixed (byte* pResult = result)
            {
                var i = 0;
                for (; i < values.Length - blockSize; i += blockSize)
                {
                    var doubles = Avx.LoadVector256(pValues + i);
                    var bytes1 = doubles.AsByte();
                    Avx.Store(pResult + (i << 3), bytes1);
                }
                
                if (i == values.Length - 1)
                {
                    return result;
                }

                // Dealing with the remainder elements
                Convert(pValues, i, values.Length, pResult);
            }
            
            return result;
        }
        
        public static unsafe byte[] GetBytesVectorsPipelined(double[] values)
        {
            if (!Avx.IsSupported) throw new Exception("AVX is not supported");

            var blockSize = 4; // 4 vectors into the pipeline
            var byteVectorSize = Vector256<byte>.Count;
            var doubleVectorSize = Vector256<double>.Count; 
            var batchSize = doubleVectorSize * blockSize;
            if (values.Length < batchSize) throw new Exception("Give me more doubles!");

            var result = new byte[values.Length * sizeof(double)];

            fixed (double* pValues = values)
            fixed (byte* pResult = result)
            {
                var i = 0;
                for (; i < values.Length - batchSize; i += batchSize)
                {
                    // CPU dependent pipelining
                    var vector1 = Avx.LoadVector256(pValues + i);
                    var vector2 = Avx.LoadVector256(pValues + i + blockSize);
                    var vector3 = Avx.LoadVector256(pValues + i + blockSize * 2);
                    var vector4 = Avx.LoadVector256(pValues + i + blockSize * 3);

                    var offset = i * sizeof(double);
                    Avx.Store(pResult + offset, vector1.AsByte());
                    Avx.Store(pResult + offset + byteVectorSize, vector2.AsByte());
                    Avx.Store(pResult + offset + byteVectorSize * 2, vector3.AsByte());
                    Avx.Store(pResult + offset + byteVectorSize * 3, vector4.AsByte());
                }
                
                if (i == values.Length - 1)
                {
                    return result;
                }

                // Dealing with the remainder elements
                Convert(pValues, i, values.Length, pResult);
            }
            
            return result;
        }

        private static unsafe void Convert(double* pValues, int start, int end, byte* pResult)
        {
            var buffer = stackalloc byte[8];
            for (var i = start; i < end; i++)
            {
                *(double*)buffer = pValues[i];
                var index = i << 3;
                pResult[index    ] = buffer[0];
                pResult[index + 1] = buffer[1];
                pResult[index + 2] = buffer[2];
                pResult[index + 3] = buffer[3];
                pResult[index + 4] = buffer[4];
                pResult[index + 5] = buffer[5];
                pResult[index + 6] = buffer[6];
                pResult[index + 7] = buffer[7];
            }
        }
    }
}