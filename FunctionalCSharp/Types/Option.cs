using System.Collections.Generic;

namespace FunctionalCSharp.Types
{
    public sealed class Option
    {
        private Option() { }

        public static Option<T> Some<T>(T value)
            => new Option<T>(value);

        public static Option None { get; } = new Option();
    }

    public sealed class Option<T>
    {
        public T Value { get; }

        private readonly bool hasValue;

        public bool IsSome => hasValue;

        public bool IsNone => !hasValue;

        public Option(T value)
        {
            Value = value;
            hasValue = true;
        }

        private Option() { }

        private static Option<T> None 
            => new Option<T>();

        public static bool operator ==(Option<T> a, Option<T> b)
            => (a.hasValue == b.hasValue) && EqualityComparer<T>.Default.Equals(a.Value, b.Value);

        public static bool operator !=(Option<T> a, Option<T> b)
            => !(a == b);

        public static implicit operator Option<T>(Option option)
            => None;

        public override int GetHashCode()
            => hasValue.GetHashCode() ^ (hasValue ? Value.GetHashCode() : 0);

        public override bool Equals(object obj)
            => base.Equals(obj);
    }
}
