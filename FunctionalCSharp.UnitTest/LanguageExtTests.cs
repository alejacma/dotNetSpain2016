using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LanguageExt.Trans;
using static LanguageExt.Prelude;

namespace FunctionalCSharp.UnitTest
{
    [TestClass]
    public class LanguageExtTests
    {
        #region "42. C# Functional Language Extensions"

        // List pattern matching
        public int Sum(IEnumerable<int> list) =>
            match(list,
                  () => 0,
                  (x, xs) => x + Sum(xs));

        [TestCategory("42. C# Functional Language Extensions")]
        [TestMethod]
        public void TestCSharpFunctionalLanguageExtensions()
        {
            // Tuple
            var name = Tuple("Alejandro", "Campos");
            var res = map(name, (first, last) => $"{first} {last}");
            Assert.AreEqual("Alejandro Campos", res);

            // Option
            var optional = Some(123);
            var num = match(optional,
                            Some: v => v * 2,
                            None: () => 0);
            Assert.AreEqual(num, 246);

            // Monad transformers
            var list = List(Some(1), None, Some(2), None, Some(3));
            var presum = list.SumT();
            Assert.AreEqual(6, presum);

            list = list.MapT(x => x * 2);
            var postsum = list.SumT();
            Assert.AreEqual(12, postsum);

            // Lack of lambda and expression inference
            var add = fun((int x, int y) => x + y);
            Assert.AreEqual(7, add(4, 3));

            // Immutable lists
            var test = List(1, 2, 3, 4, 5).Map(x => x * 10).Filter(x => x > 20).Fold(0, (x, s) => s + x);
            Assert.AreEqual(120, test);

            // List pattern matching
            var sum = Sum(List(10, 20, 30, 40, 50));
            Assert.AreEqual(150, sum);
        }

        #endregion
    }
}
