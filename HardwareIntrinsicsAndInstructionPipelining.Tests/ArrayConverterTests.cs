using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HardwareIntrinsicsAndInstructionPipelining.Tests
{
    [TestClass]
    public class ArrayConverterTests
    {
        [TestMethod]
        public void GetBytes()
        {
            var actual = ArrayConverter.GetBytes(Data.Doubles);
            
            CollectionAssert.AreEqual(Data.Bytes, actual);
        }
        
        [TestMethod]
        public void GetBytesVectors()
        {
            var actual = ArrayConverter.GetBytesVectors(Data.Doubles);
            
            CollectionAssert.AreEqual(Data.Bytes, actual);
        }
        
        [TestMethod]
        public void GetBytesVectorsPipelined()
        {
            var actual = ArrayConverter.GetBytesVectorsPipelined(Data.Doubles);
            
            CollectionAssert.AreEqual(Data.Bytes, actual);
        }
    }
}