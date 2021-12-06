namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    using ReactiveUI;
    using Soatech.Blazor.Leaflet.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class MarkerViewModel : ReactiveObject, IAsyncDisposable
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = "";
        private LatLngViewModel _position = new(new(0, 0));
        private bool _isDraggable = true;
        private float _opacity = 1.0f;

        public string Id 
        { 
            get => _id;
            set => _id = value;
        }

        public LatLngViewModel Position
        {
            get => _position;
            set
            {
                if (_position?.Value == value?.Value) return;
                if (_position != null)
                    _position.PropertyChanged -= RaisePositionChanged;
                _position = value;
                _position.PropertyChanged += RaisePositionChanged;
                this.RaisePropertyChanged();
            }
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

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private void RaisePositionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(Position));
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
