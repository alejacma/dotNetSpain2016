using FunctionalCSharp.Types;
using System;

namespace FunctionalCSharp.Extensions
{
    public static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value) => new Just<T>(value);

        public static Maybe<B> Bind<A, B>(this Maybe<A> a, Func<A, Maybe<B>> func)
        {
            var justa = a as Just<A>;
            return justa == null 
                ? new Nothing<B>() 
                : func(justa.Value);
        }

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A, B, C> select) 
            =>  a.Bind(             aval => 
                func(aval).Bind(    bval => 
                select(aval, bval).ToMaybe()));

        public static Maybe<int> Div(this int numerator, int denominator)
            => denominator == 0
                ? (Maybe<int>)new Nothing<int>()
                : new Just<int>(numerator / denominator);
    }
}
