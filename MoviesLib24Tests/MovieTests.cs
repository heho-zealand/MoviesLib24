using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesLib24;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesLib24.Tests
{
    [TestClass()]
    public class MovieTests
    {
        private readonly Movie _movie = new(1, "A", 1895);

        [TestMethod]
        public void MovieTest()
        {
            Assert.AreEqual(1, _movie.Id);
            Assert.AreEqual("A", _movie.Title);
            Assert.AreEqual(1895, _movie.Year);

            Assert.ThrowsException<ArgumentNullException>(() => _movie.Title = null);
            Assert.ThrowsException<ArgumentException>(() => _movie.Title = "");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _movie.Year = 1894);
        }

        [TestMethod]
        public void NoArgConstructorTest()
        {
            var movie = new Movie();
            Assert.AreEqual(0, movie.Id);
            Assert.AreEqual("Unknown", movie.Title);
            Assert.AreEqual(1900, movie.Year);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual("1 A 1895", _movie.ToString());
        }

    }
}