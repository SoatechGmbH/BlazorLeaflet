namespace Soatech.Blazor.Leaflet.Layers
{
    using System.Drawing;
    using Soatech.Blazor.Leaflet.Configuration;
    using Soatech.Blazor.Leaflet.Models;

    public partial class Marker : InteractiveLayer
    {
        private LatLng _position = new(0, 0);
        private CustomIconOptions _icon = new CustomIconOptions();
        private bool _keyboard = true;
        private string _title = "";
        private string _alt = "";
        private int _zIndexOffset = 0;
        private double _opacity = 1.0;
        private bool _riseOnHover = false;
        private int _riseOffset = 250;
        private string _pane = "markerPane";
        private string _shadowPane = "shadowPane";
        private bool _draggable = false;
        private bool _autoPan = false;
        private PointF _autoPanPadding = new PointF(50, 50);
        private int _autoPanSpeed = 10;

        /// <summary>
        /// constructor
        /// </summary>
        public Marker()
        {
            BubblingMouseEvents = false;
        }

        /// <summary>
        /// gets or sets the marker position on the map.
        /// </summary>
        [Parameter]
        public LatLng Position
        {
            get => _position;
            set
            {
                if (_position == value) return;

                _position = value;
                RaisePropertyChanged(nameof(Position));
            }
        }

        /// <summary>
        /// Option for rendering a custom icon. If not specified the default values are used.
        /// </summary>
        [Parameter]
        public CustomIconOptions Icon 
        {
            get => _icon; 
            set => this.SetAndRaiseIfChanged(ref _icon, value);
        }

        /// <summary>
        /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
        /// </summary>
        [Parameter]
        public bool Keyboard 
        {
            get => _keyboard;
            set => this.SetAndRaiseIfChanged(ref _keyboard, value);
        }

        /// <summary>
        /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
        /// </summary>
        [Parameter]
        public string Title
        {
            get => _title;
            set => this.SetAndRaiseIfChanged(ref _title, value);
        }

        /// <summary>
        /// Text for the alt attribute of the icon image (useful for accessibility).
        /// </summary>
        [Parameter]
        public string Alt
        {
            get => _alt;
            set => this.SetAndRaiseIfChanged(ref _alt, value);
        }

        /// <summary>
        /// By default, marker images zIndex is set automatically based on its latitude. 
        /// Use this option if you want to put the marker on top of all others (or below), 
        /// specifying a high value like 1000 (or high negative value, respectively).
        /// </summary>
        [Parameter]
        public int ZIndexOffset
        {
            get => _zIndexOffset;
            set => this.SetAndRaiseIfChanged(ref _zIndexOffset, value);
        }

        /// <summary>
        /// The opacity of the marker.
        /// </summary>
        [Parameter]
        public double Opacity
        {
            get => _opacity;
            set => this.SetAndRaiseIfChanged(ref _opacity, value);
        }

        /// <summary>
        /// If true, the marker will get on top of others when you hover the mouse over it.
        /// </summary>
        [Parameter]
        public bool RiseOnHover
        {
            get => _riseOnHover;
            set => this.SetAndRaiseIfChanged(ref _riseOnHover, value);
        }

        /// <summary>
        /// The z-index offset used for the riseOnHover feature.
        /// </summary>
        [Parameter]
        public int RiseOffset
        {
            get => _riseOffset;
            set => this.SetAndRaiseIfChanged(ref _riseOffset, value);
        }

        /// <summary>
        /// Map pane where the markers icon will be added.
        /// </summary>
        [Parameter]
        public string Pane
        {
            get => _pane;
            set => this.SetAndRaiseIfChanged(ref _pane, value);
        }

        /// <summary>
        /// Map pane where the markers shadow will be added.
        /// </summary>
        [Parameter]
        public string ShadowPane
        {
            get => _shadowPane;
            set => this.SetAndRaiseIfChanged(ref _shadowPane, value);
        }

        /// <summary>
        /// Whether the marker is draggable with mouse/touch or not.
        /// </summary>
        [Parameter]
        public bool Draggable
        {
            get => _draggable;
            set => this.SetAndRaiseIfChanged(ref _draggable, value);
        }

        /// <summary>
        /// Whether to pan the map when dragging this marker near its edge or not.
        /// </summary>
        [Parameter]
        public bool AutoPan
        {
            get => _autoPan;
            set => this.SetAndRaiseIfChanged(ref _autoPan, value);
        }

        /// <summary>
        /// Distance (in pixels to the left/right and to the top/bottom) of the map edge to start panning the map.
        /// </summary>
        [Parameter]
        public PointF AutoPanPadding
        {
            get => _autoPanPadding;
            set => this.SetAndRaiseIfChanged(ref _autoPanPadding, value);
        }

        /// <summary>
        /// Number of pixels the map should pan by.
        /// </summary>
        [Parameter]
        public int AutoPanSpeed
        {
            get => _autoPanSpeed;
            set => this.SetAndRaiseIfChanged(ref _autoPanSpeed, value);
        }

        [Parameter]
        public Action<LatLng>? PositionChanged { get; set; }

        [Parameter]
        public Action<Icon>? IconChangedChanged { get; set; }

        [Parameter]
        public Action<bool>? KeyboardChanged { get; set; }

        [Parameter]
        public Action<string>? TitleChanged { get; set; }

        [Parameter]
        public Action<string>? AltChanged { get; set; }

        [Parameter]
        public Action<int>? ZIndexOffsetChanged { get; set; }

        [Parameter]
        public Action<double>? OpacityChanged { get; set; }

        [Parameter]
        public Action<bool>? RiseOnHoverChanged { get; set; }

        [Parameter]
        public Action<int>? RiseOffsetChanged { get; set; }

        [Parameter]
        public Action<string>? PaneChanged { get; set; }

        [Parameter]
        public Action<string>? ShadowPaneChanged { get; set; }

        [Parameter]
        public Action<bool>? DraggableChanged { get; set; }

        [Parameter]
        public Action<bool>? AutoPanChanged { get; set; }

        [Parameter]
        public Action<PointF>? AutoPanPaddingChanged { get; set; }

        [Parameter]
        public Action<int>? AutoPanSpeedChanged { get; set; }

        protected override void OnInitialized()
        {
            AsyncDisposables.Add(this.WhenChanged(nameof(Position))
                .Select(_ => Position)
                .Subscribe(c => PositionChanged?.Invoke(c)));

            AsyncDisposables.Add(this.WhenChanged(nameof(Position))
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(_ => Position)
                .SelectMany(p => SetLatLng(p).AsTask().ToObservable())
                .Subscribe());

            base.OnInitialized();
        }

        protected override ValueTask<IJSObjectReference?> CreateNative()
        {
            var options = new MarkerOptions
            {
                Position = Position,
                Icon = Icon,
                Keyboard = Keyboard,
                Title = Title,
                Alt = Alt,
                ZIndexOffset = ZIndexOffset,
                Opacity = Opacity,
                RiseOnHover = RiseOnHover,
                RiseOffset = RiseOffset,
                Pane = Pane,
                ShadowPane = ShadowPane,
                Draggable = Draggable,
                AutoPan = AutoPan,
                AutoPanPadding = AutoPanPadding,
                AutoPanSpeed = AutoPanSpeed
            };
            return ParentMap.CreateMarker(options, this);
        }
    }
}
