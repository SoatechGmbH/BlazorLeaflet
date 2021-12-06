namespace Soatech.Blazor.Leaflet.Layers
{
    public partial class Layer
    {
        public ValueTask Remove()
        {
            return NativeLayer?.InvokeVoidAsync("remove") ?? ValueTask.CompletedTask;
        }

        public ValueTask AddTo(SoaMap map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            return NativeLayer?.InvokeVoidAsync("addTo", map.NativeMap) ?? ValueTask.CompletedTask;
        }

        public ValueTask AddTo(LayerGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            return NativeLayer?.InvokeVoidAsync("addTo", group.NativeLayer) ?? ValueTask.CompletedTask;
        }
    }
}
