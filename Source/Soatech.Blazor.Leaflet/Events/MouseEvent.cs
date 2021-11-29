using Soatech.Blazor.Leaflet.Models;
using System.Drawing;

namespace Soatech.Blazor.Leaflet.Events
{
    public class MouseEvent : Event
    {
        public LatLng LatLng { get; set; }

        public PointF LayerPoint { get; set; }

        public PointF ContainerPoint { get; set; }

    }
}
