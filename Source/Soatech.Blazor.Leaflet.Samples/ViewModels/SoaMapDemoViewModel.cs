using ReactiveUI;
using Soatech.Blazor.Leaflet.Models;

namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    public class SoaMapDemoViewModel : ReactiveObject
    {
        private LatLng _center = new LatLng(0, 10);
        private float _zoom = 4.0f;
        private float _minZoom = 2.0f;
        private float _maxZoom = 20.0f;
        private string _tileLayer = "https://{s}.tile.openstreetmap.de/{z}/{x}/{y}.png";

        public LatLng Center
        {
            get => _center;
            set => this.RaiseAndSetIfChanged(ref _center, value);
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
    }
}
