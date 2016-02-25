using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionalCSharp.Extensions
{
    public static class GeneralExtensions
    {
        public static void Use<T>(this T obj, Action<T> action) where T : IDisposable
        {
            using (obj)
            {
                action(obj);
            }
        }

        public static Func<T1, T2, TResult> SetDefaultArgument<T1, T2, T3, TResult>(
            this Func<T1, T2, T3, TResult> function,
            T3 defaultZ)
                => (x, y) => function(x, y, defaultZ);

        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(
            this Func<T1, T2, T3, TResult> function)
                => a => b => c => function(a, b, c);

        public static Func<T1, T2, T3, TResult> UnCurry<T1, T2, T3, TResult>(
            this Func<T1, Func<T2, Func<T3, TResult>>> curriedFunc)
                => (a, b, c) => curriedFunc(a)(b)(c);

        public static Func<X, Z> Compose<X, Y, Z>(this Func<X, Y> func1, Func<Y, Z> func2)
            => (x) => func2(func1(x));

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> list, Converter<T, R> converter)
        {
            foreach (T value in list)
            {
                yield return converter(value);
            }
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Predicate<T> condition)
        {
            foreach (T val in list)
            {
                if (condition(val))
                {
                    yield return val;
                }
            }
        }

        // (((((0 + 1) + 2) + 3) + 4) + 5)
        public static TResult FoldLeft<T, TResult>(
            this IEnumerable<T> list, TResult seed, Func<TResult, T, TResult> accumulator)
        {
            TResult result = seed;
            list.ForEach(value => result = accumulator(result, value));
            return result;
        }

        // (1 + (2 + (3 + (4 + (5 + 0)))))
        public static TResult FoldRight<T, TResult>(
            this IEnumerable<T> list, TResult seed, Func<T, TResult, TResult> accumulator)
                => list.Reverse().FoldLeft(seed, (result, value) => accumulator(value, result));

        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> function)
        {
            var results = new Dictionary<T, TResult>();
            return arg =>
            {
                TResult result;
                if (!results.TryGetValue(arg, out result))
                {
                    result = results[arg] = function(arg);
                }
                return result;
            };
        }

        public static void Require<T>(this T param, Func<T, bool> condition)
        {
            if (!condition(param))
            {
                throw new ArgumentException();
            }
        }

        public static Tuple<T, long> MeasureTime<T>(Func<T> f)
        {
            var st = Stopwatch.StartNew();
            var res = f();
            var t = st.ElapsedMilliseconds;
            return Tuple.Create(res, t);
        }
    }
}
