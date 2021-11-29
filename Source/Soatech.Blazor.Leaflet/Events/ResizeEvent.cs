using System.Drawing;

namespace Soatech.Blazor.Leaflet.Events
{
    public class ResizeEvent : Event
    {
        public PointF OldSize { get; set; }
        public PointF NewSize { get; set; }
    }
}
