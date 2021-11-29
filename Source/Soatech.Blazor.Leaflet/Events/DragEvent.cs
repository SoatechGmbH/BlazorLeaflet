using Soatech.Blazor.Leaflet.Models;

namespace Soatech.Blazor.Leaflet.Events
{
    public class DragEvent : Event
    {
        public LatLng LatLng { get; set; }

        public LatLng OldLatLng { get; set; }
    }
}
