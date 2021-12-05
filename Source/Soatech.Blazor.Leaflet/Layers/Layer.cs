namespace Soatech.Blazor.Leaflet.Layers
{
    using System.Runtime.CompilerServices;

    public abstract partial class Layer : ComponentBase, IAsyncDisposable, INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();

        protected CompositeAsyncDisposable AsyncDisposables { get; } = new CompositeAsyncDisposable();

        public event PropertyChangedEventHandler? PropertyChanged;

        [Parameter]
        public string Id 
        {
            get => _id;
            set => _id = value;
        }

        [CascadingParameter(Name = nameof(ParentMap))]
        public SoaMap? ParentMap { get; set; }

        [CascadingParameter(Name = nameof(ParentGroup))]
        public GroupLayer? ParentGroup { get; set; }

        protected IJSObjectReference? NativeLayer { get; set; }

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
            AsyncDisposables.Add(NativeLayer);
        }

        protected abstract ValueTask<IJSObjectReference?> CreateNative();

        protected override void OnInitialized()
        {
            if (ParentMap == null) throw new InvalidOperationException("Parent may not be null.");
            ParentMap.MapLayers.Add(this);
            base.OnInitialized();
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                if (ParentMap?.MapLayers != null && ParentMap.MapLayers.Contains(this))
                    ParentMap.MapLayers.Remove(this);
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
    }
}
