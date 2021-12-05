namespace System.Reactive.Disposables
{
    public class CompositeAsyncDisposable : IAsyncDisposable
    {
        private List<IAsyncDisposable> _asyncDisposables = new List<IAsyncDisposable>();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public async ValueTask DisposeAsync()
        {
            await ClearAsync();
        }

        public void Add(IAsyncDisposable disposable)
        {
            _asyncDisposables.Add(disposable);
        }
        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public async ValueTask ClearAsync()
        {
            await Task.WhenAll(_asyncDisposables.Select(d => d.DisposeAsync().AsTask()));
            _asyncDisposables.Clear();
            _disposables.Clear();
        }

        public async ValueTask<bool> RemoveAsync(IAsyncDisposable disposable)
        {
            if (!_asyncDisposables.Contains(disposable))
                return false;

            await disposable.DisposeAsync();
            return _asyncDisposables.Remove(disposable);
        }

        public bool RemoveAsync(IDisposable disposable)
        {
            return _disposables.Remove(disposable);
        }
    }
}
