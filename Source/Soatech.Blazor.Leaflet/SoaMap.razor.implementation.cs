namespace Soatech.Blazor.Leaflet
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Soatech.Blazor.Leaflet.Events;
    using Soatech.Blazor.Leaflet.Models;

    using Layer = Layers.Layer;

    public partial class SoaMap : INotifyPropertyChanged, IAsyncDisposable
    {
        #region Variables

        private readonly string _cssStyle = "soamap";
        private readonly CompositeAsyncDisposable _asyncDisposables = new();
        private readonly ObservableCollection<Layer> _layers = new();

        private string _id = $"{Guid.NewGuid()}";
        private float _minZoom = 2;
        private float _maxZoom = 10;
        private float _zoom = 8;
        private LatLng _center = new(0, 0);
        private LatLngBounds? _maxBounds;
        private LatLngBounds? _bounds;
        private bool _isInitialized;
        private event Action<bool> _onInitialized;

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Parameters

        [Parameter]
        public string Id
        {
            get => _id;
            set => _id = value;
        }

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
        public LatLngBounds? MaxBounds
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
        public Action<LatLngBounds?>? MaxBoundsChanged { get; set; }

        [Parameter]
        public LatLngBounds? Bounds
        {
            get => _bounds;
            set
            {
                if (_bounds == value) return;

                _bounds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bounds)));
            }
        }

        [Parameter]
        public Action<LatLngBounds?>? BoundsChanged { get; set; }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnContextMenu { get; set; }

        [Parameter]
        public RenderFragment Layers { get; set; }

        #endregion

        public IJSObjectReference? NativeMap { get; private set; }

        public ObservableCollection<Layer> MapLayers => _layers;

        public IObservable<Unit> OnInitialized => Observable.FromEvent<bool>(e => _onInitialized += e, e => _onInitialized -= e)
            .StartWith(_isInitialized)
            .Where(x => x == true)
            .Select(_ => Unit.Default)
            .FirstAsync();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeJsMap();
                HookParameterChanged();

                _asyncDisposables.Add(this.WhenChanged(nameof(Center))
                    .Merge(this.WhenChanged(nameof(CurrentZoom)))
                    .Merge(this.WhenChanged(nameof(MinZoom)))
                    .Merge(this.WhenChanged(nameof(MaxZoom)))
                    .Merge(this.WhenChanged(nameof(Bounds)))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ =>
                        FlyTo(Center, CurrentZoom)
                        .AsTask()
                        .ToObservable()
                    )
                    .Subscribe());

                _asyncDisposables.Add(this.WhenChanged(nameof(MaxBounds))
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .SelectMany(_ => FlyTo(Center, CurrentZoom).AsTask().ToObservable())
                    .Subscribe());

                await SetView(Center, CurrentZoom);
                await HookNativeEvents();

                _asyncDisposables.Add(
                    MapLayers.WhenCollectionChanged()
                    .Where(e => e.Action != NotifyCollectionChangedAction.Move)
                    .StartWith(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems: MapLayers, oldItems: new List<Layer>()))
                    .Select(e =>
                    (
                        Type: e.Action,
                        OldItems: e.OldItems?.Cast<Layer>() ?? Enumerable.Empty<Layer>(),
                        NewItems: e.NewItems?.Cast<Layer>() ?? Enumerable.Empty<Layer>()
                    ))
                    .SelectMany(e =>
                        Task.WhenAll(
                            e.OldItems.Select(itm => itm.DisposeAsync().AsTask()))
                        .ToObservable()
                        .Select(_ => e))
                    .SelectMany(e =>
                        Task.WhenAll(
                            e.NewItems.Select(itm => itm.Create().AsTask()))
                        .ToObservable()
                        .Select(_ => e))
                    .Subscribe(e => 
                    {
                        if (e.Type == NotifyCollectionChangedAction.Replace)
                        {
                            _isInitialized = true;
                            _onInitialized?.Invoke(_isInitialized);
                        }
                    }));

                _asyncDisposables.Add(
                    this.OnInitialized
                    .Subscribe(_ => this.PropertyChanged?.Invoke(this, new(nameof(Center)))));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void HookParameterChanged()
        {
            _asyncDisposables.Add(this.WhenChanged(nameof(Center))
                .Select(_ => Center)
                .Subscribe(c => CenterChanged?.Invoke(c)));

            _asyncDisposables.Add(this.WhenChanged(nameof(MinZoom))
                .Select(_ => MinZoom)
                .Subscribe(mz => MinZoomChanged?.Invoke(mz)));

            _asyncDisposables.Add(this.WhenChanged(nameof(MaxZoom))
                .Select(_ => MaxZoom)
                .Subscribe(mz => MaxZoomChanged?.Invoke(mz)));

            _asyncDisposables.Add(this.WhenChanged(nameof(CurrentZoom))
                .Select(_ => CurrentZoom)
                .Subscribe(cz => CurrentZoomChanged?.Invoke(cz)));

            _asyncDisposables.Add(this.WhenChanged(nameof(ShowZoomControl))
                .Select(_ => ShowZoomControl)
                .Subscribe(sz => ShowZoomControlChanged?.Invoke(sz)));

            _asyncDisposables.Add(this.WhenChanged(nameof(ShowAttributionControl))
                .Select(_ => ShowAttributionControl)
                .Subscribe(ac => ShowAttributionControlChanged?.Invoke(ac)));

            _asyncDisposables.Add(this.WhenChanged(nameof(MaxBounds))
                .Select(_ => MaxBounds)
                .Subscribe(mb => MaxBoundsChanged?.Invoke(mb)));

            _asyncDisposables.Add(this.WhenChanged(nameof(Bounds))
                .Select(_ => Bounds)
                .Subscribe(b => BoundsChanged?.Invoke(b)));
        }

        public async ValueTask DisposeAsync()
        {
            await _asyncDisposables.DisposeAsync();
            await DisposeInteropAsync();
            GC.SuppressFinalize(this);
        }
    }
}
