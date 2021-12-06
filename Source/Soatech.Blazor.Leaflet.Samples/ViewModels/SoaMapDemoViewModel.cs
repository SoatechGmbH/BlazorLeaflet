using ReactiveUI;
using Soatech.Blazor.Leaflet.Models;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    public class SoaMapDemoViewModel : ReactiveObject, IAsyncDisposable
    {
        private CompositeAsyncDisposable _disposables = new();
        private LatLngViewModel _center = new(new LatLng(0, 10));
        private LatLngViewModel _markerPosition = new(new LatLng(10, 10));
        private float _zoom = 4.0f;
        private float _minZoom = 2.0f;
        private float _maxZoom = 20.0f;
        private string _tileLayer = "https://{s}.tile.openstreetmap.de/{z}/{x}/{y}.png";
        private LatLngBounds _bounds = new LatLngBounds();
        private ObservableCollection<LayerViewModel> _layers = new();
        private ObservableCollection<MarkerViewModel> _selectedMarkers = new();
        private Random _random = new Random((int)DateTime.Now.Ticks);

        public SoaMapDemoViewModel()
        {
            _disposables.Add(Center.WhenChanged().Subscribe(_ => this.RaisePropertyChanged(nameof(Center))));

            for (int i = 0; i < 5; i++)
            {
                var layer = new LayerViewModel
                {
                    Name = $"Layer {i + 1}",
                    IsVisible = i == 0 ? true : false
                };

                _disposables.Add(layer.WhenChanged().Subscribe(_ => this.RaisePropertyChanged(nameof(Layers))));

                for (int m = 0; m < 100; m++)
                {
                    var marker = new MarkerViewModel
                    {
                        Position = new(new((_random.NextSingle() * 180) - 90, (_random.NextSingle() * 360) - 180)),
                        Opacity = _random.NextSingle(),
                        Name = $"{layer.Name} - Marker {m}"
                    };
                    marker.IsDraggable = marker.Opacity > 0.5f;
                    
                    layer.AddMarker(marker);
                }

                Layers.Add(layer);
            }
        }

        public LatLngViewModel Center
        {
            get => _center;
            set => this.RaiseAndSetIfChanged(ref _center, value);
        }

        public LatLngViewModel MarkerPosition
        {
            get => _markerPosition;
            set => this.RaiseAndSetIfChanged(ref _markerPosition, value);
        }

        public ObservableCollection<LayerViewModel> Layers
        {
            get => _layers;
            set => this.RaiseAndSetIfChanged(ref _layers, value);
        }

        public ObservableCollection<MarkerViewModel> SelectedMarkers
        {
            get => _selectedMarkers;
            set => this.RaiseAndSetIfChanged(ref _selectedMarkers, value);
        }

        public float MinZoom
        {
            get => _minZoom;
            set => this.RaiseAndSetIfChanged(ref _minZoom, value);
        }
        public float MaxZoom
        {
            get => _maxZoom;
            set => this.RaiseAndSetIfChanged(ref _maxZoom, value);
        }

        public float Zoom
        {
            get => _zoom;
            set => this.RaiseAndSetIfChanged(ref _zoom, value);
        }

        public string TileLayer
        {
            get => _tileLayer;
            set => this.RaiseAndSetIfChanged(ref _tileLayer, value);
        }

        public LatLngBounds Bounds
        {
            get => _bounds;
            set => this.RaiseAndSetIfChanged(ref _bounds, value);
        }

        public void SelectMarker(MarkerViewModel marker)
        {
            if (SelectedMarkers.Contains(marker))
                SelectedMarkers.Remove(marker);
            else
                SelectedMarkers.Add(marker);
        }

        public ValueTask DisposeAsync()
        {
            return _disposables?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}
