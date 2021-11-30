namespace Soatech.Blazor.Leaflet.Layers
{
    using Microsoft.JSInterop;
    using Soatech.Blazor.Leaflet.Configuration;
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;

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

        protected override async ValueTask OnCreateNativeComponent()
        {
            var config = new TileLayerOptions()
            {
                UrlTemplate = TileSourceTemplate,
                TileSize = 512,
                ZoomOffset = -1,
                Attribution = "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors, Imagery © <a href=\"https://www.openstreetmap.org/\">OpenStreetMap</a>"
            };

            _nativeLayer = await Parent.CreateTileLayer(config, this);

            Disposables.Add(this.WhenChanged(nameof(TileSourceTemplate))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(_ => TileSourceTemplate)
                .SelectMany(t => SetUrl(t).AsTask().ToObservable())
                .Subscribe());
        }
    }
}
