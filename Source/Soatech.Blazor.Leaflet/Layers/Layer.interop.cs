namespace Soatech.Blazor.Leaflet.Layers
{
    public partial class Layer
    {
        public ValueTask Remove()
        {
            return NativeLayer?.InvokeVoidAsync("remove") ?? ValueTask.CompletedTask;
        }
    }
}
