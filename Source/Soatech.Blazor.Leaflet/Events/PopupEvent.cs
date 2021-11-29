using Soatech.Blazor.Leaflet.Models;

namespace Soatech.Blazor.Leaflet.Events
{
    public class PopupEvent : Event
    {
        public Popup Popup { get; set; }
    }
}
