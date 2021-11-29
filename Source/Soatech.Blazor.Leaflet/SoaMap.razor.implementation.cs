global using System;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Components;
using Soatech.Blazor.Leaflet.Configuration;
using Soatech.Blazor.Leaflet.Events;
using Soatech.Blazor.Leaflet.Models;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Soatech.Blazor.Leaflet
{
    public partial class SoaMap : ComponentBase, INotifyPropertyChanged, IDisposable
    {
        private readonly string _id = $"{Guid.NewGuid()}";
        private readonly string _cssStyle = "soamap";
        private readonly CompositeDisposable _disposables = new();

        private string _tileSourceTemplate = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
        private float _minZoom = 2;
        private float _maxZoom = 10;
        private float _zoom = 8;
        private LatLng _center = new(0, 0);

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Id => _id;

        [Parameter]
        public bool ShowZoomControl { get; set; } = true;

        [Parameter]
        public Action<bool>? ShowZoomControlChanged { get; set; }

        [Parameter]
        public bool ShowAttributionControl { get; set; } = true;

        [Parameter]
        public Action<bool>? ShowAttributionControlChanged { get; set; }

        [Parameter]
        public float MinZoom 
        {
            get => _minZoom;
            set
            {
                if (_minZoom == value) return;

                _minZoom = value;
                if (_minZoom > _maxZoom)
                    _minZoom = _maxZoom;
                if (_minZoom < 0)
                    _minZoom = 0;
                if (_zoom < _minZoom)
                    CurrentZoom = _minZoom;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinZoom)));
            }
        }

        [Parameter]
        public Action<float>? MinZoomChanged { get; set; }

        [Parameter]
        public float MaxZoom
        {
            get => _maxZoom;
            set
            {
                if (_maxZoom == value) return;

                _maxZoom = value;
                if (_maxZoom < _minZoom)
                    _maxZoom = _minZoom;
                if (_zoom > _maxZoom)
                    CurrentZoom = _maxZoom;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxZoom)));
            }
        }

        [Parameter]
        public Action<float>? MaxZoomChanged { get; set; }

        [Parameter]
        public float CurrentZoom 
        {
            get => _zoom;
            set
            {
                if (_zoom == value) return;

                _zoom = value;
                if (_zoom < _minZoom)
                    _zoom = _minZoom;
                if (_zoom > _maxZoom)
                    _zoom = _maxZoom;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentZoom)));
            }
        }

        [Parameter]
        public Action<float>? CurrentZoomChanged { get; set; }

        [Parameter]
        public LatLng Center 
        {
            get => _center;
            set
            {
                if (_center == value) return;

                _center = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Center)));
            }
        }

        [Parameter]
        public Action<LatLng>? CenterChanged { get; set; }

        [Parameter]
        public string TileSourceTemplate
        {
            get => _tileSourceTemplate;
            set
            {
                if (_tileSourceTemplate == value) return;

                _tileSourceTemplate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TileSourceTemplate)));
            }
        }

        [Parameter]
        public Action<string>? TileSourceTemplateChanged { get; set; }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnConextMenu { get; set; }

        protected override void OnInitialized()
        {
            _disposables.Add(this.WhenChanged(nameof(Center))
                .Select(_ => Center)
                .Subscribe(c => CenterChanged?.Invoke(c)));

            _disposables.Add(this.WhenChanged(nameof(MinZoom))
                .Select(_ => MinZoom)
                .Subscribe(z => MinZoomChanged?.Invoke(z)));
            
            _disposables.Add(this.WhenChanged(nameof(MaxZoom))
                .Select(_ => MaxZoom)
                .Subscribe(z => MaxZoomChanged?.Invoke(z)));
            
            _disposables.Add(this.WhenChanged(nameof(CurrentZoom))
                .Select(_ => CurrentZoom)
                .Subscribe(z => CurrentZoomChanged?.Invoke(z)));
            
            _disposables.Add(this.WhenChanged(nameof(ShowZoomControl))
                .Select(_ => ShowZoomControl)
                .Subscribe(z => ShowZoomControlChanged?.Invoke(z)));
            
            _disposables.Add(this.WhenChanged(nameof(ShowAttributionControl))
                .Select(_ => ShowAttributionControl)
                .Subscribe(z => ShowAttributionControlChanged?.Invoke(z)));

            _disposables.Add(this.WhenChanged(nameof(TileSourceTemplate))
                .Select(_ => TileSourceTemplate)
                .Subscribe(z => TileSourceTemplateChanged?.Invoke(z)));

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeJsMap();

                var config = new TileLayerConfiguration()
                {
                    UrlTemplate = TileSourceTemplate,
                    TileSize = 512,
                    ZoomOffset = -1,
                    Attribution = "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors, Imagery © <a href=\"https://www.openstreetmap.org/\">OpenStreetMap</a>"
                };

                await CreateTileLayer(config);

                _disposables.Add(this.WhenChanged(nameof(Center))
                    .Merge(this.WhenChanged(nameof(CurrentZoom)))
                    .Merge(this.WhenChanged(nameof(MinZoom)))
                    .Merge(this.WhenChanged(nameof(MaxZoom)))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ => FlyTo(Center, CurrentZoom))
                    .Subscribe());

                _disposables.Add(this.WhenChanged(nameof(TileSourceTemplate))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ => 
                    {
                        config.UrlTemplate = TileSourceTemplate;
                        return CreateTileLayer(config);
                     })
                    .Subscribe());

                await SetView(Center, CurrentZoom);
                await HookEvents();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnButtonClicked()
        {
            await FitWorld();
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
