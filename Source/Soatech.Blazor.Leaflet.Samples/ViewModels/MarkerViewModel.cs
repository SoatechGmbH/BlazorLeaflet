namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    using ReactiveUI;
    using Soatech.Blazor.Leaflet.Models;
    using System.Threading.Tasks;

    public class MarkerViewModel : ReactiveObject, IAsyncDisposable
    {
        private string _id = Guid.NewGuid().ToString();
        private LatLng _position = new(0, 0);
        private bool _isDraggable = true;
        private float _opacity = 1.0f;

        public string Id 
        { 
            get => _id;
            set => _id = value;
        }

        public LatLng Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        public bool IsDraggable
        {
            get => _isDraggable;
            set => this.RaiseAndSetIfChanged(ref _isDraggable, value);
        }

        public float Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
