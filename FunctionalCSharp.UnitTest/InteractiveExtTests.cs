using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FunctionalCSharp.UnitTest
{
    [TestClass]
    public class InteractiveExtTests
    {
        #region "41. Interactive Extensions"

        [TestCategory("41. Interactive Extensions")]
        [TestMethod]
        public void TestInteractiveExtensions()
        {
            // Lazily invoke an action
            Console.WriteLine("Do:");
            var numbers = new[] { 30, 40, 20, 40 };
            var result = numbers.Do(Console.WriteLine);

            Console.WriteLine("Before Enumeration");
            result.ForEach(n => { }); // The action will be invoked when enumerating
            Console.WriteLine("After Enumeration");

            // Generate a sequence by repeating the source while the condition is true
            Console.WriteLine("DoWhile:");
            var then = DateTime.Now.Add(new TimeSpan(0, 0, 1));
            result = numbers.DoWhile(() => DateTime.Now < then);

            Console.WriteLine("Before Enumeration");
            result.ForEach(Console.WriteLine);
            Console.WriteLine("After Enumeration");

            // Generate a sequence of non-overlapping adjacent buffers over the source sequence
            var results = numbers.Buffer(3).ToArray();
            Assert.AreEqual(2, results.Count());
            CollectionAssert.AreEqual(new[] { 30, 40, 20 }, results[0].ToArray());
            CollectionAssert.AreEqual(new[] { 40 }, results[1].ToArray());

            // Apply an accumulator to generate a sequence of accumulated values
            result = numbers.Scan(0, (sum, num) => sum + num); // 0 is the seed

            CollectionAssert.AreEqual(new[] { 0, 30, 70, 90, 130 }, result.ToArray());
        }

        #endregion
    }
}
