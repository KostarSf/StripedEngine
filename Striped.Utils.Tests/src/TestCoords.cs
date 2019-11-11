using System;
using NUnit.Framework;

namespace Striped.Utils.Tests
{
    [TestFixture]
    public class TestCoords
    {

        [Test]
        public void HorisontalDistance_NegativeNegative_3return()
        {
            var result = Coords.HorisontalDistance(new Coords(-6, 0), new Coords(-3, 0));

            Assert.AreEqual(result, 3);
        }

        [Test]
        public void HorisontalDistance_NegativeNegativeReverse_3return()
        {
            var result = Coords.HorisontalDistance(new Coords(-3, 0), new Coords(-6, 0));

            Assert.AreEqual(result, 3);
        }

        [Test]
        public void HorisontalDistance_NegativePositive_9return()
        {
            var result = Coords.HorisontalDistance(new Coords(-3, 0), new Coords(6, 0));

            Assert.AreEqual(result, 9);
        }

        [Test]
        public void HorisontalDistance_NegativeZero_9return()
        {
            var result = Coords.HorisontalDistance(new Coords(-9, 0), new Coords(0, 0));

            Assert.AreEqual(result, 9);
        }

        [Test]
        public void HorisontalDistance_ZeroPositive_9return()
        {
            var result = Coords.HorisontalDistance(new Coords(0, 0), new Coords(9, 0));

            Assert.AreEqual(result, 9);
        }

        [Test]
        public void HorisontalDistance_PositivePositive_9return()
        {
            var result = Coords.HorisontalDistance(new Coords(3, 0), new Coords(12, 0));

            Assert.AreEqual(result, 9);
        }

        [Test]
        public void GetPath_Test()
        {
            var path = Coords.GetPath(new Coords(1, 5), new Coords(0, 0));

            var output = "";
            var expect = "1, 5;\n1, 4;\n1, 3;\n0, 2;\n0, 1;\n0, 0;\n";

            foreach (var coord in path)
            {
                output += $"{coord.X}, {coord.Y};\n";
            }

            Assert.AreEqual(output, expect);
        }
    }
}