using FunctionalCSharp.Types;

namespace FunctionalCSharp.Extensions
{
    public static class OptionExtensions
    {
        public static Option<T> ToOption<T>(this T value)
            => Option.Some(value);
    }
}
