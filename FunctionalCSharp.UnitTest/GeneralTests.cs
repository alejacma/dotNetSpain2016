using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FunctionalCSharp.UnitTest
{
    using FunctionalCSharp.Extensions;
    using FunctionalCSharp.Types;
    using HelperFSharp;
    using static FunctionalCSharp.Types.Option;
    using static FunctionalCSharp.Extensions.GeneralExtensions;
    using static FunctionalCSharp.Extensions.MaybeExtensions;
    using System.IO;

    [TestClass]
    public class GeneralTests
    {
        // First-class Functions in C#

        #region "01. Function Types"

        // Strongly typed delegate
        public delegate double MyFunction(double x);

        [TestCategory("01. Function Types")]
        [TestMethod]
        public void TestStronglyTypedDelegate()
        {
            MyFunction f = Math.Sin;
            double y = f(Math.PI / 2);

            Assert.AreEqual(1, y);

            f = Math.Exp;
            y = f(1);

            Assert.AreEqual(Math.E, y);
        }

        [TestCategory("01. Function Types")]
        [TestMethod]
        public void TestGenericFunctionTypes()
        {
            // Generic function type
            Func<double, double> f = Math.Sin;
            double y = f(Math.PI / 2);

            Assert.AreEqual(1, y);

            f = Math.Exp;
            y = f(1);

            Assert.AreEqual(Math.E, y);

            // Predicate type
            Predicate<string> isEmptyString = string.IsNullOrEmpty;
            bool isEmpty = isEmptyString("Test");

            Assert.AreEqual(false, isEmpty);

            // Action type
            Action<string> println = Console.WriteLine;
            println("Test");
        }

        #endregion

        #region "02. Function Values"

        [TestCategory("02. Function Values")]
        [TestMethod]
        public void TestAssignValuesToFunctions()
        {
            // Point to existing method by name
            Func<double, double> f1 = Math.Sin;
            Func<double, double> f2 = Math.Exp;
            double y = f1(Math.PI / 2) + f2(1);

            Assert.AreEqual(1 + Math.E, y);

            // Pass function to another function
            f2 = f1;
            y = f2(Math.PI / 2);

            Assert.AreEqual(1, y);
        }

        [TestCategory("02. Function Values")]
        [TestMethod]
        public void TestAnonymousFunctions()
        {
            // Anonymous delegate
            Func<double, double> f = delegate (double x) { return 3 * x + 1; };
            double y = f(4);

            Assert.AreEqual(13, y);

            // Lambda Expression
            f = x => 3 * x + 1;
            y = f(5);

            Assert.AreEqual(16, y);
        }

        #endregion

        #region "03. Lambda Expressions"

        [TestCategory("03. Lambda Expressions")]
        [TestMethod]
        public void TestExpressionLambdas()
        {

            Func<int> f1 = delegate () { return 3; };
            f1 = () => 3;

            Func<DateTime> f2 = delegate () { return DateTime.Now; };
            f2 = () => DateTime.Now;

            Func<int, int> f3 = delegate (int x) { return x + 1; };
            f3 = x => x + 1;

            Func<int, double> f4 = delegate (int x) { return Math.Log(x + 1) - 1; };
            f4 = (x) => Math.Log(x + 1) - 1;

            Func<int, int, int> f5 = delegate (int x, int y) { return x + y; };
            f5 = (x, y) => x + y;

            Func<string, string, string> f6 = delegate (string x, string y) { return $"{x} {y}"; };
            f6 = (x, y) => $"{x} {y}";
        }

        [TestCategory("03. Lambda Expressions")]
        [TestMethod]
        public void TestStatementLambda()
        {
            Action<string> myDel = n =>
                {
                    string s = $"{n} World";
                    Console.WriteLine(s);
                };

            myDel("Hello");
        }

        #endregion

        #region "04. Functions as Parameters"

        public void Operate(int x, int y, Func<int, int, int> operation)
        {
            Console.WriteLine(operation(x, y));
        }

        [TestCategory("04. Functions as Parameters")]
        [TestMethod]
        public void TestFunctionsAsParameters()
        {
            Operate(5, 6, (x, y) => x + y);
            Operate(5, 6, (x, y) => x * y);
        }

        #endregion

        #region "05. Function Arithmetic"

        public static void Hello(string s)
        {
            Console.WriteLine($"Hello, {s}!");
        }

        public static void Goodbye(string s)
        {
            Console.WriteLine($"  Goodbye, {s}!");
        }

        [TestCategory("05. Function Arithmetic")]
        [TestMethod]
        public void TestFuntionArithmetic()
        {
            Action<string> action = Console.WriteLine;
            Action<string> hello = Hello;
            Action<string> goodbye = Goodbye;
            action += Hello;
            action += (x) => Console.WriteLine($"  Greating {x} from lambda expression");
            action("First");

            action -= hello;
            action("Second");

            action = Console.WriteLine + goodbye
                    + delegate (string x)
                    {
                        Console.WriteLine($"  Greating {x} from delegate");
                    };
            action("Third");

            (action - Goodbye)("Fourth");

            action.GetInvocationList().ToList().ForEach(del => Console.WriteLine(del.Method.Name));
        }

        #endregion

        #region "06. Higher Order Functions"

        public Func<X, Z> Compose<X, Y, Z>(Func<X, Y> f, Func<Y, Z> g)
        {
            return (x) => g(f(x));
        }

        [TestCategory("06. Higher Order Functions")]
        [TestMethod]
        public void TestHigherOrderFunction()
        {
            Func<double, double> sin = Math.Sin;
            Func<double, double> exp = Math.Exp;
            Func<double, double> expSin = Compose(sin, exp);
            double y = expSin(3);

            Assert.AreEqual(1.151562836514535, y);
        }

        #endregion

        #region "07. Expression-bodied Members"

        public int OldSum(int a, int b)
        {
            return a + b;
        }

        public int NewSum(int a, int b) => a + b;

        public string OldName
        {
            get
            {
                return "Alejandro Campos Magencio";
            }
        }

        public string NewName => "Alejandro Campos Magencio";

        [TestCategory("07. Expression-bodied Members")]
        [TestMethod]
        public void TestExpressionBodiedMembers()
        {
            Assert.AreEqual(5, NewSum(2, 3));
            Assert.AreEqual("Alejandro Campos Magencio", NewName);
        }

        #endregion

        #region "08. Immutable Types: Tuples"

        private Random rnd = new Random();

        public Tuple<double, double> RandomPrice(double max)
            => Tuple.Create(Math.Round(rnd.NextDouble() * max, 1), Math.Round(rnd.NextDouble() * max, 1));

        public Tuple<double, double> Normalize(Tuple<double, double> price)
            => price.Item1 < price.Item2 ? price : Tuple.Create(price.Item2, price.Item1);

        [TestCategory("08. Immutable Types: Tuples")]
        [TestMethod]
        public void TestTuples()
        {
            Tuple<double, double> price = Normalize(RandomPrice(100.0));

            Assert.AreEqual(true, price.Item1 < price.Item2);
        }

        #endregion

        #region "09. Closures"

        public Tuple<Action, Action, Action> CreateBoundFunctions()
        {
            int val = 0;

            Action increment = () => val++;
            Action decrement = () => val--;
            Action print = () => Console.WriteLine($"val = {val}");

            return Tuple.Create(increment, decrement, print);
        }

        [TestCategory("09. Closures")]
        [TestMethod]
        public void TestBoundFunctions()
        {
            var functions = CreateBoundFunctions();
            var increment = functions.Item1;
            var decrement = functions.Item2;
            var print = functions.Item3;

            increment();
            print();
            increment();
            print();
            increment();
            print();
            decrement();
            print();
        }

        #endregion

        #region "10. Expression Trees"

        [TestCategory("10. Expression Trees")]
        [TestMethod]
        public void TestExpressionTree()
        {
            // Create
            Expression<Func<int, bool>> exprTree = num => num < 5;

            // Decompose
            ParameterExpression param = exprTree.Parameters[0];
            BinaryExpression operation = (BinaryExpression)exprTree.Body;
            ParameterExpression left = (ParameterExpression)operation.Left;
            ConstantExpression right = (ConstantExpression)operation.Right;

            Assert.AreEqual("num => num LessThan 5", $"{param.Name} => {left.Name} {operation.NodeType} {right.Value}");

            // Compile
            Func<int, bool> result = exprTree.Compile();

            // Invoke
            Assert.AreEqual(true, result(4));
        }

        #endregion

        #region "11. Async Functions"

        public int SlowSum(int x, int y)
        {
            Thread.Sleep(10000);
            return x + y;
        }

        [TestCategory("11. Async Functions")]
        [TestMethod]
        public void TestSlowFunction()
        {
            Func<int, int, int> f = SlowSum;

            // Start execution
            IAsyncResult async = f.BeginInvoke(5, 3, null, null);

            int sum;

            // Check is function completed
            if (async.IsCompleted)
            {
                sum = f.EndInvoke(async);
            }

            //Finally demand result
            sum = f.EndInvoke(async);

            Assert.AreEqual(8, sum);
        }

        #endregion

        #region "12. Task Parallel Library (TPL)" 

        [TestCategory("12. Task Parallel Library (TPL)")]
        [TestMethod]
        public void TestLambdasInParallel()
        {
            Console.WriteLine("Let's go!");

            var task1 = Task.Run(() => SlowSum(3, 4));
            var task2 = Task.Run(() => SlowSum(5, 6));

            Console.WriteLine("Doing something else in the meantime");

            Console.WriteLine($"Result 1 = {task1.Result}, Result 2 = {task2.Result}");

            Assert.AreEqual(7, task1.Result);
            Assert.AreEqual(11, task2.Result);
        }

        #endregion

        #region "13. Async Lambdas"

        public async Task<string> ExampleMethodAsync()
        {
            // The following line simulates a task-returning asynchronous process.
            await Task.Delay(1000);
            return "Ok";
        }

        [TestCategory("13. Async Lambdas")]
        [TestMethod]
        public async Task TestExampleMethodAsync()
        {
            string result = await ExampleMethodAsync();

            Assert.AreEqual("Ok", result);
        }

        [TestCategory("13. Async Lambdas")]
        [TestMethod]
        public async Task TestExampleMethodWithLambdaAsync()
        {
            Func<Task<string>> f = async () => await ExampleMethodAsync();

            string result = await f();

            Assert.AreEqual("Ok", result);
        }

        #endregion

        #region "14. Generics"

        public class GenericList<T>
        {
            void Add(T input) { }
        }

        public class ExampleClass { }

        [TestCategory("14. Generics")]
        [TestMethod]
        public void TestGenericType()
        {
            var list1 = new GenericList<int>();
            var list2 = new GenericList<string>();
            var list3 = new GenericList<ExampleClass>();
        }

        #endregion

        #region "15. Type Inference & Anonymous Types"

        [TestCategory("15. Type Inference & Anonymous Types")]
        [TestMethod]
        public void TestTypeInference()
        {
            var v = new { Amount = 108, Message = "Hello" };

            Assert.AreEqual(108, v.Amount);
            Assert.AreEqual("Hello", v.Message);
        }

        #endregion

        #region "16. Generic Methods"

        public int Count<T>(T[] arr, Predicate<T> condition)
        {
            int counter = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (condition(arr[i]))
                {
                    counter++;
                }
            }
            return counter;
        }

        [TestCategory("16. Generic Methods")]
        [TestMethod]
        public void TestCount()
        {
            var words = new string[] { "Title", "Some Long Tile", "" };

            var numberOfBooksWithLongNames = Count(words, word => word.Length > 10);
            var numberOfEmptyBookTitles = Count(words, string.IsNullOrEmpty);

            var numbers = new int[] { 3, 4, -3, 0, -1 };

            var numberOfNegativeNumbers = Count(numbers, x => x < 0);

            Assert.AreEqual(1, numberOfBooksWithLongNames);
            Assert.AreEqual(1, numberOfEmptyBookTitles);
            Assert.AreEqual(2, numberOfNegativeNumbers);
        }

        #endregion

        #region "17. Generic Delegates"

        public delegate void Del<T>(T item);

        public void Notify(int i) { }

        [TestCategory("17. Generic Delegates")]
        [TestMethod]
        public void TestGenericDelegate()
        {
            Del<int> m2 = Notify;
        }

        #endregion  

        #region "18. Extension Methods"

        public class SomeClass : IDisposable
        {
            public void Dispose() => Console.WriteLine("Object disposed!");

            public void DoAction() => Console.WriteLine("Done!");
        }

        [TestCategory("18. Extension Methods")]
        [TestMethod]
        public void TestGenericExtensionMethod()
        {
            //using (var obj = new SomeClass())
            //{
            //    obj.DoAction();
            //}

            var obj = new SomeClass();

            obj.Use(x => x.DoAction());
        }

        #endregion

        #region "19. Covariance & Contravariance"

        public class Person { }
        public class Employee : Person { }

        public Employee FindByTitle(string title) => new Employee();

        [TestCategory("19. Covariance & Contravariance")]
        [TestMethod]
        public void TestCovariance()
        {
            Func<string, Employee> findEmployee = FindByTitle;

            Func<string, Person> findPerson = FindByTitle;

            findPerson = findEmployee;
        }

        public void AddToContacts(Person person) { }

        [TestCategory("19. Covariance & Contravariance")]
        [TestMethod]
        public void TestContravariance()
        {
            Action<Person> addPersonToContacts = AddToContacts;

            Action<Employee> addEmployeeToContacts = AddToContacts;

            addEmployeeToContacts = addPersonToContacts;
        }

        // Invariant generic delegate
        public delegate T DInvariant<T>();

        // Covariant generic delegate
        public delegate R DCovariant<out R>();

        // Contravariant generic delegate
        public delegate void DContravariant<in A>(A a);

        // Variant generic delegate
        public delegate R DVariant<in A, out R>(A a);

        [TestCategory("19. Covariance & Contravariance")]
        [TestMethod]
        public void TestVariance()
        {
            DInvariant<string> dinString = () => "";
            //DInvariant<object> dinObject = dinString;

            DCovariant<string> dcoString = () => " ";
            DCovariant<object> dcoObject = dcoString;

            DContravariant<object> dcontraObject = (a) => Console.WriteLine(a);
            DContravariant<string> dcontraString = dcontraObject;

            DVariant<object, string> dObjectString = (a) => $"{a} ";
            DVariant<object, object> dObjectObject = dObjectString;
            DVariant<string, string> dStringString = dObjectString;
            DVariant<string, object> dStringObject = dObjectString;
        }

        #endregion

        #region "20. Dynamics"

        [TestCategory("20. Dynamics")]
        [TestMethod]
        public void TestDynamics()
        {
            //Func<object, object> doubleIt = p => p + p;
            Func<dynamic, dynamic> doubleIt = p => p + p;

            Assert.AreEqual(4, doubleIt(2));
            Assert.AreEqual("22", doubleIt("2"));
        }

        #endregion

        #region "21. LINQ (Language-Integrated Query)"

        [TestCategory("21. LINQ (Language-Integrated Query)")]
        [TestMethod]
        public void TestLinq()
        {
            var scores = new int[] { 60, 92, 81, 97 };

            var scoreQuery =
                from score in scores
                where score > 80
                orderby score
                select score;

            foreach (var i in scoreQuery)
            {
                Console.Write($"{i} ");
            }

            CollectionAssert.AreEqual(new[]{ 81, 92, 97 }, scoreQuery.ToList());
        }

        [TestCategory("21. LINQ (Language-Integrated Query)")]
        [TestMethod]
        public void TestLinqWithLambdas()
        {
            var scores = new int[] { 60, 92, 81, 97 };

            var scoreQuery = scores.Where(score => score > 80).OrderBy(score => score);

            foreach (var i in scoreQuery)
            {
                Console.Write($"{i} ");
            }

            CollectionAssert.AreEqual(new[] { 81, 92, 97 }, scoreQuery.ToList());
        }

        #endregion

        #region "22. Parallel LINQ (PLINQ)"

        [TestCategory("22. Parallel LINQ (PLINQ)")]
        [TestMethod]
        public void TestPLINQ()
        {
            var nums = Enumerable.Range(10, 10000);
            var query = from num in nums.AsParallel()//.AsOrdered()
                        where num % 10 == 0
                        select num;

            var concurrentBag = new ConcurrentBag<string>();
            query.ForAll(e => concurrentBag.Add($"Value = {e}"));

            Assert.AreEqual(1000, concurrentBag.Count);
        }

        [TestCategory("22. Parallel LINQ (PLINQ)")]
        [TestMethod]
        public void TestPLINQWithLambdas()
        {
            var nums = Enumerable.Range(10, 10000);
            var query = nums.AsParallel()/*.AsOrdered()*/.Where(num => num % 10 == 0);

            var concurrentBag = new ConcurrentBag<string>();
            query.ForAll(e => concurrentBag.Add($"Value = {e}"));

            Assert.AreEqual(1000, concurrentBag.Count);
        }

        #endregion

        #region "23. Lazy Listings with Iterators"

        public IEnumerable<int> MyEndlessRandomNumberGenerator()
        {
            while (true)
            {
                yield return rnd.Next();
            }
        }

        [TestCategory("23. Lazy Listings with Iterators")]
        [TestMethod]
        public void TestIterator()
        {
            MyEndlessRandomNumberGenerator().Take(5).ForEach(Console.WriteLine);
        }

        #endregion

        // Implementing Functional Techniques in C#

        #region "24. Refactoring into Pure Functions"

        private string aMember = "StringOne";

        public void HyphenatedConcat(string appendStr) => aMember += '-' + appendStr;

        public void HyphenatedConcat(StringBuilder sb, string appendStr) => sb.Append('-' + appendStr);

        public string HyphenatedConcat(string s, string appendStr) => $"{s}-{appendStr}";

        [TestCategory("24. Refactoring into Pure Functions")]
        [TestMethod]
        public void TestImpureAndPureFunctions()
        {
            // Impure
            HyphenatedConcat("StringTwo");
            Assert.AreEqual("StringOne-StringTwo", aMember);

            // Impure
            var sb1 = new StringBuilder("StringOne");
            HyphenatedConcat(sb1, "StringTwo");
            Assert.AreEqual("StringOne-StringTwo", sb1.ToString());

            // Pure
            var s1 = "StringOne";
            var s2 = HyphenatedConcat(s1, "StringTwo");
            Assert.AreEqual("StringOne", s1);
            Assert.AreEqual("StringOne-StringTwo", s2);
        }

        #endregion

        #region "25. Building Immutable Types"

        public class ProductPile
        {
            public string ProductName { get; }

            public int Amount { get; }

            public decimal Price { get; }

            public ProductPile(string productName, int amount, decimal price)
            {
                productName.Require(p => !string.IsNullOrWhiteSpace(p));
                amount.Require(a => a >= 0);
                price.Require(p => p > 0);

                ProductName = productName;
                Amount = amount;
                Price = price;
            }

            public ProductPile SubtractOne()
                => new ProductPile(ProductName, Amount - 1, Price);
        }

        [TestCategory("25. Building Immutable Types")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImmutability()
        {
            var product = new ProductPile(null, 3, 13);
        }

        [TestCategory("25. Building Immutable Types")]
        [TestMethod]
        public void TestImmutabilityWithInvalidArgument()
        {
            var product = new ProductPile("Milk", 3, 13);
            Assert.AreEqual(3, product.Amount);

            var newProduct = product.SubtractOne();
            Assert.AreEqual(3, product.Amount);
            Assert.AreEqual(2, newProduct.Amount);
        }

        #endregion

        #region "26. Immutable Types: Options"

        [TestCategory("26. Immutable Types: Options")]
        [TestMethod]
        public void TestOption()
        {
            var optInt1 = new Option<int>(10);
            var optInt2 = 10.ToOption();
            var optInt3 = Some(10);

            Assert.AreEqual(true, optInt1.IsSome);
            Assert.AreEqual(10, optInt1.Value);
            Assert.AreEqual(true, optInt1 == optInt2);
            Assert.AreEqual(true, optInt2 == optInt3);

            var optString1 = Some("test");
            var optString2 = Some<string>(null);

            Assert.AreEqual(true, optString1.IsSome);
            Assert.AreEqual("test", optString1.Value);
            Assert.AreEqual(true, optString2.IsSome);
            Assert.AreEqual(null, optString2.Value);

            optString1 = None;

            Assert.AreEqual(true, optString1.IsNone);
        }

        #endregion

        #region "27. Recursion with Lambdas"

        public IEnumerable<int> InfiniteNumbers()
        {
            for (int i = 0; true; i++)
            {
                Console.WriteLine($"Number is {i}");
                yield return i;
            }
        }

        [TestCategory("27. Recursion with Lambdas")]
        [TestMethod]
        public void TestRecursionWithLambdas()
        {
            var results = InfiniteNumbers().Take(10).Select(
                num =>
                {
                    Func<int, int> factorial = null;
                    factorial = n => n == 0 ? 1 : n * factorial(n - 1);
                    return factorial(num);
                });

            results.ForEach(result => Console.WriteLine($"Factorial is {result}"));

            CollectionAssert.AreEqual(new[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880 }, results.ToList()); 
        }

        #endregion

        #region "28. Lazy Evaluation"

        public Lazy<int> LazySum(Lazy<int> a, Lazy<int> b)
        {
            Console.WriteLine("LazySum");
            return new Lazy<int>(() => { Console.WriteLine("a + b"); return a.Value + b.Value; });
        }

        [TestCategory("28. Lazy Evaluation")]
        [TestMethod]
        public void TestLazyEvaluation()
        {
            var a = new Lazy<int>(() => { Console.WriteLine("a"); return 10 * 2; });
            var b = new Lazy<int>(() => { Console.WriteLine("b"); return 5; });

            var result = LazySum(a, b);

            Console.WriteLine("Result:");
            Console.WriteLine(result.Value);

            Assert.AreEqual(25, result.Value);
        }

        #endregion

        #region "29. Partial Functions"

        public double Distance(double x, double y, double z) => Math.Sqrt(x * x + y * y + z * z);

        [TestCategory("29. Partial Functions")]
        [TestMethod]
        public void TestPartialFunctions()
        {
            Func<double, double, double, double> distance3D = Distance;
            Func<double, double, double> distance2D = distance3D.SetDefaultArgument(0);
            var d1 = distance2D(3, 6);  // distance3D(3, 6, 0);
            var d2 = distance2D(3, 4);  // distance3D(3, 4, 0);
            var d3 = distance2D(1, 2);  // distance3D(1, 2, 0);

            Assert.AreEqual(6.7082039324993694, d1);
            Assert.AreEqual(5, d2);
            Assert.AreEqual(2.23606797749979, d3);
        }

        #endregion

        #region "30. Curry Functions"

        [TestCategory("30. Curry Functions")]
        [TestMethod]
        public void TestCurryFunction()
        {
            Func<double, double, double, double> distance3D = Distance;

            var curriedDistance = distance3D.Curry();

            var d = curriedDistance(3)(4)(12);

            Assert.AreEqual(13, d);
        }

        #endregion

        #region "31. Uncurry Functions"

        [TestCategory("31. Uncurry Functions")]
        [TestMethod]
        public void TestUnCurryFunction()
        {
            Func<double, double, double, double> distance3D = Distance;
            var curriedDistance = distance3D.Curry();

            Func<double, double, double, double> originalDistance = curriedDistance.UnCurry();

            Assert.AreEqual(13, curriedDistance(3)(4)(12));
            Assert.AreEqual(13, originalDistance(3, 4, 12));
        }

        #endregion

        #region "32. Composition"

        [TestCategory("32. Composition")]
        [TestMethod]
        public void TestComposition()
        {
            Func<int, int> calcBwithA = a => a * 3;
            Func<int, int> calcCwithB = b => b + 27;

            var calcCWithA = Compose(calcBwithA, calcCwithB);
            Assert.AreEqual(39, calcCWithA(4));

            // or...

            calcCWithA = calcBwithA.Compose(calcCwithB);
            Assert.AreEqual(39, calcCWithA(4));
        }

        #endregion

        #region "33. Map"

        [TestCategory("33. Map")]
        [TestMethod]
        public void TestMap()
        {
            var people = new List<dynamic> {
                new {Name = "Harry", Age = 32},
                new {Name = "Anna", Age = 45},
                new {Name = "Willy", Age = 43},
                new {Name = "Rose", Age = 37}
            };

            people.Map(p => p.Name).ForEach(Console.WriteLine);

            Console.WriteLine("Same thing with LinQ:");

            people.Select(p => p.Name).ForEach(Console.WriteLine);
        }

        #endregion

        #region "34. Filter"

        [TestCategory("34. Filter")]
        [TestMethod]
        public void TestFilter()
        {
            var people = new List<dynamic> {
                new {Name = "Harry", Age = 32},
                new {Name = "Anna", Age = 45},
                new {Name = "Willy", Age = 43},
                new {Name = "Rose", Age = 37}
            };

            people.Filter(p => p.Age > 40).ForEach(Console.WriteLine);

            Console.WriteLine("Same thing with LinQ:");

            people.Where(p => p.Age > 40).ForEach(Console.WriteLine);
        }

        #endregion

        #region "35. Fold Left"

        [TestCategory("35. Fold Left")]
        [TestMethod]
        public void TestFoldLeft()
        {
            var sumOf1to10 = Enumerable.Range(1, 10).FoldLeft("0", (acc, value) => $"({acc}+{value})");
            Assert.AreEqual("((((((((((0+1)+2)+3)+4)+5)+6)+7)+8)+9)+10)", sumOf1to10);

            var people = new List<dynamic> {
                new {Name = "Harry", Age = 32},
                new {Name = "Anna", Age = 45},
                new {Name = "Willy", Age = 43},
                new {Name = "Rose", Age = 37}
            };

            var result = people.FoldLeft(
                Tuple.Create(0, 0),
                (acc, value) => Tuple.Create(acc.Item1 + value.Age, acc.Item2 + 1));
            var averageAge = result.Item1 / result.Item2;
            Assert.AreEqual(39, averageAge);

            // Same thing with LinQ

            sumOf1to10 = Enumerable.Range(1, 10).Aggregate("0", (acc, value) => $"({acc}+{value})");
            Assert.AreEqual("((((((((((0+1)+2)+3)+4)+5)+6)+7)+8)+9)+10)", sumOf1to10);

            result = people.Aggregate(
                Tuple.Create(0, 0),
                (acc, value) => Tuple.Create(acc.Item1 + value.Age, acc.Item2 + 1));
            averageAge = result.Item1 / result.Item2;
            Assert.AreEqual(39, averageAge);
        }

        #endregion

        #region "36. Fold Right"

        [TestCategory("36. Fold Right")]
        [TestMethod]
        public void TestFoldRight()
        {
            var sumOf1to10 = Enumerable.Range(1, 10).FoldRight("0", (value, acc) => $"({value}+{acc})");
            Assert.AreEqual("(1+(2+(3+(4+(5+(6+(7+(8+(9+(10+0))))))))))", sumOf1to10);

            var people = new List<dynamic> {
                new {Name = "Harry", Age = 32},
                new {Name = "Anna", Age = 45},
                new {Name = "Willy", Age = 43},
                new {Name = "Rose", Age = 37}
            };

            var result = people.FoldRight(
                Tuple.Create(0, 0),
                (value, acc) => Tuple.Create(value.Age + acc.Item1, 1 + acc.Item2));
            var averageAge = result.Item1 / result.Item2;
            Assert.AreEqual(39, averageAge);

            // In LinQ, Aggregate is a left folding operator

            sumOf1to10 =
                Enumerable.Range(1, 10).
                Aggregate<int, Func<string, string>>(x => x, (f, value) => acc => f($"({value}+{acc})"))("0");
            Assert.AreEqual("(1+(2+(3+(4+(5+(6+(7+(8+(9+(10+0))))))))))", sumOf1to10);

            result =
                people.
                Aggregate<dynamic, Func<Tuple<int, int>, Tuple<int, int>>>(
                    x => x, (f, value) => acc => f(Tuple.Create(value.Age + acc.Item1, 1 + acc.Item2)))(Tuple.Create(0, 0));
            averageAge = result.Item1 / result.Item2;
            Assert.AreEqual(39, averageAge);
        }

        #endregion

        #region "37. Memoization"

        [TestCategory("37. Memoization")]
        [TestMethod]
        public void TestMemoization()
        {
            Func<long, long> fib = null;
            fib = n => n > 1 ? fib(n - 1) + fib(n - 2) : n;

            var res = MeasureTime(() => fib(40));

            Assert.AreEqual(102334155, res.Item1);
            Console.WriteLine($"Standard version took {res.Item2} milliseconds");

            fib = fib.Memoize();
            res = MeasureTime(() => fib(40));

            Assert.AreEqual(102334155, res.Item1);
            Console.WriteLine($"Memoized version took {res.Item2} milliseconds");
        }

        #endregion

        #region "38. Monads"

        public Maybe<int> Add4(int x) => (x + 4).ToMaybe();

        public Maybe<int> MultBy2(int x) => (x * 2).ToMaybe();

        public Maybe<int> JustFail(int x) => new Nothing<int>();

        [TestCategory("38. Monads")]
        [TestMethod]
        public void TestMaybeMonad()
        {
            var result = 3.ToMaybe().Bind(Add4).Bind(MultBy2);
            Assert.AreEqual("14", result.ToString());

            result = 3.ToMaybe().Bind(Add4).Bind(JustFail).Bind(MultBy2);
            Assert.AreEqual("Nothing", result.ToString());
        }

        public Maybe<int> DoSomeDivision(int denominator) 
            => from a in 12.Div(denominator)
               from b in a.Div(2)
               select b;

        [TestCategory("38. Monads")]
        [TestMethod]
        public void TestMaybeMonadWithLinQ()
        {
            var result = from a in "Hello World!".ToMaybe()
                         from b in DoSomeDivision(2)
                         from c in (new DateTime(2016, 2, 24)).ToMaybe()
                         select $"{a} {b} {c.ToShortDateString()}";
            Assert.AreEqual("Hello World! 3 24/02/2016", result.ToString());

            result = from a in "Hello World!".ToMaybe()
                     from b in DoSomeDivision(0)
                     from c in (new DateTime(2016, 2, 24)).ToMaybe()
                     select $"{a} {b} {c.ToShortDateString()}";
            Assert.AreEqual("Nothing", result.ToString());
        }

        #endregion

        #region "39. C# & F# Interop"

        [TestCategory("39. C# & F# Interop")]
        [TestMethod]
        public void TestDiscriminatedUnionsWithPatternMatching()
        {
            var shapes = new List<Shape>
            {
                Shape.NewCircle(15.0),
                Shape.NewSquare(10.0),
                Shape.NewRectangle(5.0, 10.0),
                Shape.NewEquilateralTriangle(10.0)
            };

            var areas = shapes.Select(s => s.Area()).ToArray();

            CollectionAssert.AreEqual(new[] { 706.85834715, 100, 50, 43.301270189221924 }, areas);
        }

        #endregion

        // Libraries 

        #region "40. System.Collections.Immutable"

        [TestCategory("40. System.Collections.Immutable")]
        [TestMethod]
        public void TestImmutableLists()
        {
            var list = ImmutableList.Create(1, 2, 3);

            list = Enumerable.Range(1, 3).ToImmutableList();

            var builder = ImmutableList.CreateBuilder<int>();
            builder.Add(1);
            builder.Add(2);
            builder.Add(3);
            list = builder.ToImmutable();

            var list2 = list.Add(4);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, list2);
        }

        #endregion

        // "41. Interactive Extensions" at InteractiveExtTest.cs

        // "42. C# Functional Language Extensions" at LanguagesExtTest.cs

        // Conclusions

        #region "43. Imperative vs. Functional Style"

        private void NonFunctionalExample(string text)
        {
            using (StringReader rdr = new StringReader(text))
            {
                string contents = rdr.ReadToEnd();
                string[] words = contents.Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    words[i] = words[i].Trim();
                }

                Dictionary<string, int> d = new Dictionary<string, int>();

                foreach (string word in words)
                {
                    if (d.ContainsKey(word))
                    {
                        d[word]++;
                    }
                    else
                    {
                        d.Add(word, 1);
                    }
                }

                foreach (KeyValuePair<string, int> kvp in d)
                {
                    Console.WriteLine(string.Format("({0}, {1})", kvp.Key, kvp.Value.ToString()));
                }
            }
        }

        private void FunctionalExample(string text)
            => new StringReader(text).Use(stream => stream
                .ReadToEnd()
                .Split(' ')
                .Select(str => str.Trim())
                .GroupBy(str => str)
                .Select(group => Tuple.Create(group.Key, group.Count()))
                .ForEach(Console.WriteLine));

        [TestCategory("43. Imperative vs. Functional Style")]
        [TestMethod]
        public void Test()
        {
            Console.WriteLine("NON FUNCTIONAL SAMPLE:");
            NonFunctionalExample("En un lugar de la mancha de cuyo nombre no puedo acordarme");

            Console.WriteLine("FUNCTIONAL SAMPLE:");
            FunctionalExample("En un lugar de la mancha de cuyo nombre no puedo acordarme");
        }

        #endregion
    }
}
