namespace Soatech.Blazor.Leaflet.Configuration
{
    using System.Drawing;

    public class FitBoundsOptions : PanOptions
    {
        public PointF PaddingTopLeft { get; set; } = new(0, 0);
        public PointF PaddingBottomRight { get; set; } = new(0, 0);
        public PointF Padding { get; set; } = new(0, 0);
        public float? MaxZoom { get; set; }
    }
}
