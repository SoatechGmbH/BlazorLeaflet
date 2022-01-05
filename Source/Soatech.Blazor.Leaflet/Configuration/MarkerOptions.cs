namespace Soatech.Blazor.Leaflet.Configuration
{
    using Soatech.Blazor.Leaflet.Models;
    using System.Drawing;

    public class MarkerOptions
    {
        public LatLng Position { get; set; } = new LatLng(0, 0);
        public CustomIconOptions Icon { get; set; } = new CustomIconOptions();
        public bool Keyboard { get; set; } = true;
        public string Title { get; set; } = "";
        public string Alt { get; set; } = "";
        public int ZIndexOffset { get; set; } = 0;
        public double Opacity { get; set; } = 1.0;
        public bool RiseOnHover { get; set; } = false;
        public int RiseOffset { get; set; } = 250;
        public string Pane { get; set; } = "markerPane";
        public string ShadowPane { get; set; } = "shadowPane";
        public bool Draggable { get; set; } = false;
        public bool AutoPan { get; set; } = false;
        public PointF AutoPanPadding { get; set; } = new PointF(50, 50);
        public int AutoPanSpeed { get; set; } = 10;
    }
}
