using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HaystacksNeedlesAndHardwareIntrinsics.Tests
{
    [TestClass]
    public class StringUtilsTests
    {
        [DataTestMethod]
        [DataRow("Hi there, where is the needle", "Hi there, where is the needle", 0)]
        [DataRow("Hi there, where is the needle", "the", 3)]
        [DataRow("x", "x", 0)]
        [DataRow("There is no needle here", "noNeedle", -1)]
        [DataRow("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum",
            "commodo", 213)]
        public void NaiveIndexOf(string haystack, string needle, int expectedPosition)
        {
            var actualPosition = StringUtils.NaiveIndexOf(haystack, needle);
            
            Assert.AreEqual(expectedPosition, actualPosition);
        }
        
        [DataTestMethod]
        [DataRow("Hi there, where is the needle", "Hi there, where is the needle", 0)]
        [DataRow("Hi there, where is the needle", "the", 3)]
        [DataRow("x", "x", 0)]
        [DataRow("There is no needle here", "noNeedle", -1)]
        [DataRow("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum",
            "commodo", 213)]
        public void IntrinsicsIndexOf(string haystack, string needle, int expectedPosition)
        {
            var actualPosition = StringUtils.IntrinsicsIndexOf(haystack, needle);
            
            Assert.AreEqual(expectedPosition, actualPosition);
        }
    }
}