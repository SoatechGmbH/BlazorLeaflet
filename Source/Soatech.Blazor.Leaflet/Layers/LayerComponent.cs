namespace Soatech.Blazor.Leaflet
{
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    public abstract class LayerComponent : ComponentBase, IDisposable, INotifyPropertyChanged
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public event PropertyChangedEventHandler PropertyChanged;

        [Parameter]
        public string Id { get; set; } = $"{Guid.NewGuid()}";

        [CascadingParameter]
        public SoaMap Parent { get; set; }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ValueTask CreateNativeComponent() => OnCreateNativeComponent();

        protected virtual ValueTask OnCreateNativeComponent() => ValueTask.CompletedTask;

        protected override void OnInitialized()
        {
            Parent?.MapLayers.Add(this);
            base.OnInitialized();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.MapLayers.Remove(this);
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) 
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
