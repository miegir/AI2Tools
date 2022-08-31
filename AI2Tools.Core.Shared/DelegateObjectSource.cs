namespace AI2Tools;

public static class DelegateObjectSource
{
    public static IObjectSource<T> Create<T>() where T : new() => Create(() => new T());

    public static IObjectSource<T> Create<T>(Func<T> factory) => new ObjectSource<T>(factory);

    private class ObjectSource<T> : IObjectSource<T>
    {
        private readonly Func<T> factory;

        public ObjectSource(Func<T> factory) => this.factory = factory;

        public T Deserialize() => factory();
    }
}
