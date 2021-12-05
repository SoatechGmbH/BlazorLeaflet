using ReactiveUI;
using Soatech.Blazor.Leaflet.Models;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    public class SoaMapDemoViewModel : ReactiveObject, IAsyncDisposable
    {
        private CompositeAsyncDisposable _disposables = new();
        private LatLng _center = new LatLng(0, 10);
        private LatLng _markerPosition = new LatLng(10, 10);
        private float _zoom = 4.0f;
        private float _minZoom = 2.0f;
        private float _maxZoom = 20.0f;
        private string _tileLayer = "https://{s}.tile.openstreetmap.de/{z}/{x}/{y}.png";
        private ObservableCollection<MarkerViewModel> _markers = new();
        private Random _random = new Random((int)DateTime.Now.Ticks);

        public SoaMapDemoViewModel()
        {
            for (int i = 0; i < 100; i++)
            {
                var marker = new MarkerViewModel
                {
                    Position = new((_random.NextSingle()*180)-90, (_random.NextSingle() * 360) - 180),
                    Opacity = _random.NextSingle()
                };
                marker.IsDraggable = marker.Opacity > 0.5f;

                Markers.Add(marker);
            }
        }

        public LatLng Center
        {
            get => _center;
            set => this.RaiseAndSetIfChanged(ref _center, value);
        }

        public LatLng MarkerPosition
        {
            get => _markerPosition;
            set => this.RaiseAndSetIfChanged(ref _markerPosition, value);
        }

        public float MarkerPositionLat
        {
            get => _markerPosition.Lat;
            set
            {
                if (_markerPosition.Lat == value) return;

                var position = new LatLng(value, _markerPosition.Lng);
                _markerPosition = position;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(MarkerPosition));
            }
        }

        public float MarkerPositionLng
        {
            get => _markerPosition.Lng;
            set
            {
                if (_markerPosition.Lng == value) return;

                var position = new LatLng(_markerPosition.Lat, value);
                _markerPosition = position;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(MarkerPosition));
            }
        }

        public ObservableCollection<MarkerViewModel> Markers
        {
            get => _markers;
            set => this.RaiseAndSetIfChanged(ref _markers, value);
        }

        public float CenterLat
        {
            get => _center.Lat;
            set
            {
                if (_center.Lat == value) return;

                var center = new LatLng(value, _center.Lng);
                _center = center;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(Center));
            }
        }

        public float CenterLng
        {
            get => _center.Lng;
            set
            {
                if (_center.Lng == value) return;

                var center = new LatLng(_center.Lat, value);
                _center = center;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(Center));
            }
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

        public ValueTask DisposeAsync()
        {
            return _disposables?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}
