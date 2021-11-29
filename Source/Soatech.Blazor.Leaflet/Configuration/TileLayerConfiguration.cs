namespace Soatech.Blazor.Leaflet.Configuration
{
    internal class TileLayerConfiguration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UrlTemplate { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

        public string Attribution { get; set; } = "";

        public float MinZoom { get; set; } = 1;

        public float MaxZoom { get; set; } = 20;

        public ushort TileSize { get; set; } = 128;

        public int ZoomOffset { get; set; } = 0;
    }
}
