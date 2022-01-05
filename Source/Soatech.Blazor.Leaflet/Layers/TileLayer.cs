namespace Soatech.Blazor.Leaflet.Layers
{
    using Soatech.Blazor.Leaflet.Configuration;

    public partial class TileLayer : GridLayer
    {
        private string _tileSourceTemplate = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
        private IJSObjectReference? _nativeLayer;

        [Parameter]
        public string TileSourceTemplate
        {
            get => _tileSourceTemplate;
            set
            {
                if (_tileSourceTemplate == value) return;

                _tileSourceTemplate = value;
                RaisePropertyChanged();
            }
        }

        [Parameter]
        public Action<string>? TileSourceTemplateChanged { get; set; }

        protected override void OnInitialized()
        {
            AsyncDisposables.Add(this.WhenChanged(nameof(TileSourceTemplate))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(_ => TileSourceTemplate)
                .SelectMany(t => SetUrl(t).AsTask().ToObservable())
                .Subscribe());

            base.OnInitialized();
        }

        protected override ValueTask<IJSObjectReference?> CreateNative()
        {
            var config = new TileLayerOptions()
            {
                UrlTemplate = TileSourceTemplate,
                TileSize = 256,
                ZoomOffset = 0,
                Attribution = "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors, Imagery © <a href=\"https://www.openstreetmap.org/\">OpenStreetMap</a>"
            };

            return ParentMap.CreateTileLayer(config, this);
        }
    }
}
