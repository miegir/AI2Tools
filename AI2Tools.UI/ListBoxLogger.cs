using System.Text;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class ListBoxLogger : ILogger
{
    private readonly ListBox listBox;
    private IDisposable? scope;

    public ListBoxLogger(ListBox listBox)
    {
        this.listBox = listBox;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        while (true)
        {
            var currentScope = scope;
            var newScope = new LoggerScope<TState>(this, state, currentScope);

            if (Interlocked.CompareExchange(ref scope, newScope, currentScope) == currentScope)
            {
                return newScope;
            }
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var currentScope = scope;
        listBox.BeginInvoke(() =>
        {
            var line = $"[{logLevel}] {currentScope}{formatter(state, exception)}";
            var index = listBox.Items.Add(line);
            listBox.SelectedIndex = index;
        });
    }

    private class LoggerScope<TState> : IDisposable
    {
        private readonly ListBoxLogger owner;
        private readonly TState state;
        private readonly IDisposable? parent;

        public LoggerScope(ListBoxLogger owner, TState state, IDisposable? parent = null)
        {
            this.owner = owner;
            this.state = state;
            this.parent = parent;
        }

        public void Dispose()
        {
            Interlocked.CompareExchange(ref owner.scope, parent, this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (parent is not null)
            {
                builder.Append(parent);
            }

            return builder.Append(state).Append(": ").ToString();
        }
    }
}
