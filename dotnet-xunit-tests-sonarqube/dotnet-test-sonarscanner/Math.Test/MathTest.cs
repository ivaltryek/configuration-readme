using System;
using Math.Api;
using Xunit;

namespace Math.Test {

    public class MathTest {
        [Theory]
        [InlineData (2, 3, 5)]
        [InlineData (5, 6, 11)]
        public void AddTest (int firstValue, int secondValue, int expected) {
            SimpleMath math = new SimpleMath ();
            int actual = math.add (firstValue, secondValue);
            Assert.Equal (expected, actual);

        }

        [Theory]
        [InlineData (5, 2, 3)]
        [InlineData (7, 8, -1)]
        public void SubstractTest (int firstValue, int secondValue, int expected) {
            SimpleMath math = new SimpleMath ();
            int actual = math.substract (firstValue, secondValue);
            Assert.Equal (expected, actual);

        }

        [Theory]
        [InlineData (10, 2, 5)]
        public void DivideTest (int firstValue, int secondValue, int expected) {
            SimpleMath math = new SimpleMath ();
            decimal actual = math.divide (firstValue, secondValue);
            Assert.Equal (expected, actual);

        }

        [Fact]
        public void DivideTest_ThrowsException () {
            SimpleMath math = new SimpleMath ();
            Assert.Throws<DivideByZeroException> (() => math.divide (10, 0));
        }
    }
}