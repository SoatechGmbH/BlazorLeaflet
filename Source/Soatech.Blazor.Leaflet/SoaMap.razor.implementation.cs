global using System;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Components;
using Soatech.Blazor.Leaflet.Configuration;
using Soatech.Blazor.Leaflet.Events;
using Soatech.Blazor.Leaflet.Models;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soatech.Blazor.Leaflet
{
    public partial class SoaMap : ComponentBase, INotifyPropertyChanged, IDisposable
    {
        #region Variables

        private readonly string _cssStyle = "soamap";
        private readonly CompositeDisposable _disposables = new();
        private readonly ObservableCollection<LayerComponent> _layers = new();

        private float _minZoom = 2;
        private float _maxZoom = 10;
        private float _zoom = 8;
        private LatLng _center = new(0, 0);
        private (LatLng sw, LatLng ne)? _maxBounds;

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Parameters

        [Parameter]
        public string Id { get; set; } = $"{Guid.NewGuid()}";

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
        public (LatLng sw, LatLng ne)? MaxBounds
        {
            get => _maxBounds;
            set
            {
                if (_maxBounds == value) return;

                _maxBounds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxBounds)));
            }
        }

        [Parameter]
        public Action<(LatLng sw, LatLng ne)?>? MaxBoundsChanged { get; set; }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnConextMenu { get; set; }

        [Parameter]
        public RenderFragment Layers { get; set; }

        #endregion

        public ObservableCollection<LayerComponent> MapLayers => _layers;

        protected override void OnInitialized()
        {
            _disposables.Add(this.WhenChanged(nameof(Center))
                .Select(_ => Center)
                .Subscribe(c => CenterChanged?.Invoke(c)));

            _disposables.Add(this.WhenChanged(nameof(MinZoom))
                .Select(_ => MinZoom)
                .Subscribe(mz => MinZoomChanged?.Invoke(mz)));
            
            _disposables.Add(this.WhenChanged(nameof(MaxZoom))
                .Select(_ => MaxZoom)
                .Subscribe(mz => MaxZoomChanged?.Invoke(mz)));
            
            _disposables.Add(this.WhenChanged(nameof(CurrentZoom))
                .Select(_ => CurrentZoom)
                .Subscribe(cz => CurrentZoomChanged?.Invoke(cz)));
            
            _disposables.Add(this.WhenChanged(nameof(ShowZoomControl))
                .Select(_ => ShowZoomControl)
                .Subscribe(sz => ShowZoomControlChanged?.Invoke(sz)));
            
            _disposables.Add(this.WhenChanged(nameof(ShowAttributionControl))
                .Select(_ => ShowAttributionControl)
                .Subscribe(ac => ShowAttributionControlChanged?.Invoke(ac)));

            _disposables.Add(this.WhenChanged(nameof(MaxBounds))
                .Select(_ => MaxBounds)
                .Subscribe(mb => MaxBoundsChanged?.Invoke(mb)));

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeJsMap();

                _disposables.Add(this.WhenChanged(nameof(Center))
                    .Merge(this.WhenChanged(nameof(CurrentZoom)))
                    .Merge(this.WhenChanged(nameof(MinZoom)))
                    .Merge(this.WhenChanged(nameof(MaxZoom)))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ => 
                        FlyTo(Center, CurrentZoom)
                        .AsTask()
                        .ToObservable()
                    )
                    .Subscribe());


                foreach (var layer in MapLayers)
                    await layer.CreateNativeComponent();

                _disposables.Add(this.WhenChanged(nameof(MaxBounds))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ => FlyTo(Center, CurrentZoom).AsTask().ToObservable())
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
            foreach (var layer in MapLayers)
                layer?.Dispose();

            _disposables?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
