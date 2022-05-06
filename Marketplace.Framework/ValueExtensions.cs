namespace Marketplace.Framework
{
    public static class ValueExtensions
    {
        public static void MustNotBeNull<T>(this Value<T> value) where T : Value<T>
        {
            if (value is null)
                throw new InvalidValueException(typeof(T), "cannot be null");
        }

        public static void MustBe<T>(this Value<T> value) where T : Value<T>
        {
            if (value is null)
                throw new InvalidValueException(typeof(T), "cannot be null");
        }
    }
}