namespace Soatech.Blazor.Leaflet.Layers
{
    public partial class LayerGroup
    {
        public ValueTask AddLayer(Layer layer)
        {
            return NativeLayer?.InvokeVoidAsync("addLayer", layer.NativeLayer) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetName(string name)
        {
            return NativeLayer?.InvokeVoidAsync("setName", name) ?? ValueTask.CompletedTask;
        }
    }
}
