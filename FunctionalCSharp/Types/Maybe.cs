namespace FunctionalCSharp.Types
{
    public interface Maybe<T> { }

    public class Nothing<T> : Maybe<T>
    {
        public override string ToString() => "Nothing";
    }

    public class Just<T> : Maybe<T>
    {
        public T Value { get; }

        public Just(T value) { Value = value; }

        public override string ToString() => Value.ToString();
    }
}
