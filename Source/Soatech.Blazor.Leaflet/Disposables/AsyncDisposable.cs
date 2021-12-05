namespace System.Reactive.Disposables
{
    using System;
    using System.Threading.Tasks;

    public class AsyncDisposable : IAsyncDisposable
    {
        private readonly Func<ValueTask> _onDispose;

        protected AsyncDisposable(Func<ValueTask> onDispose)
        {
            _onDispose = onDispose;
        }

        public static IAsyncDisposable Create(Func<ValueTask> onDispose)
        {
            return new AsyncDisposable(onDispose);
        }

        public ValueTask DisposeAsync()
        {
            return _onDispose();
        }
    }
}
