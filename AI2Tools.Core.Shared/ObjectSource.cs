namespace AI2Tools;

public static class ObjectSource
{
    public static IObjectSource<T> Create<T>() where T : new() => Create(() => new T());
    public static IObjectSource<T> Create<T>(T value) => new ConstObjectSource<T>(value);
    public static IObjectSource<T> Create<T>(Func<T> factory) => new DelegateObjectSource<T>(factory);

    private class ConstObjectSource<T> : IObjectSource<T>
    {
        private readonly T value;
        public ConstObjectSource(T value) => this.value = value;
        public T Deserialize() => value;
    }

    private class DelegateObjectSource<T> : IObjectSource<T>
    {
        private readonly Func<T> factory;
        public DelegateObjectSource(Func<T> factory) => this.factory = factory;
        public T Deserialize() => factory();
    }
}
