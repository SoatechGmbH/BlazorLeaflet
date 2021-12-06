namespace Soatech.Blazor.Leaflet.Layers
{
    using System.Runtime.CompilerServices;

    public abstract partial class Layer : ComponentBase, IAsyncDisposable, INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private bool _isVisible = true;

        protected CompositeAsyncDisposable AsyncDisposables { get; } = new CompositeAsyncDisposable();

        public event PropertyChangedEventHandler? PropertyChanged;

        [Parameter]
        public string Id 
        {
            get => _id;
            set => _id = value;
        }

        [Parameter]
        public bool IsVisible
        {
            get => _isVisible;
            set => SetAndRaiseIfChanged(ref _isVisible, value);
        }

        [Parameter]
        public Action<bool>? IsVisibleChanged { get; set; }


        [CascadingParameter(Name = nameof(ParentMap))]
        public SoaMap? ParentMap { get; set; }

        [CascadingParameter(Name = nameof(ParentGroup))]
        public LayerGroup? ParentGroup { get; set; }

        public IJSObjectReference? NativeLayer { get; protected set; }

        public async ValueTask DisposeAsync()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask Create()
        {
            NativeLayer = await CreateNative();
            AsyncDisposables.Add(AsyncDisposable.Create(() => Remove()));
            AsyncDisposables.Add(AsyncDisposable.Create(DisposeNativeLayerAsync));
            await AddToParentLayer();
        }

        protected abstract ValueTask<IJSObjectReference?> CreateNative();

        protected override void OnInitialized()
        {
            if (ParentMap == null) throw new InvalidOperationException("Parent may not be null.");
            if (ParentGroup == null)
                ParentMap.MapLayers.Add(this);
            else
                ParentGroup.MapLayers.Add(this);

            AsyncDisposables.Add(this.WhenChanged(nameof(IsVisible))
                .Select(_ => IsVisible)
                .Subscribe(ac => IsVisibleChanged?.Invoke(ac)));

            AsyncDisposables.Add(
                this.WhenChanged(nameof(IsVisible))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(_ => IsVisible)
                .Select(v => v
                    ? AddToParentLayer().AsTask().ToObservable()
                    : Remove().AsTask().ToObservable())
                .Switch()
                .Subscribe()
            );

            base.OnInitialized();
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                if (ParentMap?.MapLayers != null && ParentMap.MapLayers.Contains(this))
                    ParentMap.MapLayers.Remove(this);
                if (ParentGroup?.MapLayers != null && ParentGroup.MapLayers.Contains(this))
                    ParentGroup.MapLayers.Remove(this);
                await AsyncDisposables.DisposeAsync();
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) 
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetAndRaiseIfChanged<T>(ref T backingField, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (object.Equals(backingField, newValue))
                return false;

            backingField = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private ValueTask AddToParentLayer()
        {
            if (!IsVisible) return ValueTask.CompletedTask;

            if (ParentGroup == null)
                return AddTo(ParentMap);
            else
                return AddTo(ParentGroup);
        }

        private ValueTask DisposeNativeLayerAsync()
        {
            var nativeLayer = NativeLayer;
            NativeLayer = null;
            return NativeLayer?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}
