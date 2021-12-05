namespace Soatech.Blazor.Leaflet.Layers
{
    using Soatech.Blazor.Leaflet.Events;
    using Soatech.Blazor.Leaflet.Models;

    public partial class Marker
    {
        public ValueTask SetLatLng(LatLng position)
        {
            Position = position;
            return NativeLayer?.InvokeVoidAsync("setLatLng", position) ?? ValueTask.CompletedTask;
        }

        [JSInvokable]
        public ValueTask NotifyPositionChanged(ValueEvent<LatLng> args)
        {
            Position = args.Value ?? new();
            return ValueTask.CompletedTask;
        }
    }
}
