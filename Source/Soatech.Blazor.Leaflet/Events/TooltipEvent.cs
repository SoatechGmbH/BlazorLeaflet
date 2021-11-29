using Soatech.Blazor.Leaflet.Models;

namespace Soatech.Blazor.Leaflet.Events
{
    public class TooltipEvent : Event
    {
        public Tooltip Tooltip { get; set; }
    }
}
