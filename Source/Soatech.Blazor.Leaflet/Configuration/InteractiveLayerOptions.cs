namespace Soatech.Blazor.Leaflet.Configuration
{
    /// <summary>
    /// Options for an interactive Layer
    /// </summary>
    public class InteractiveLayerOptions
    {
        /// <summary>
        /// If false, the layer will not emit mouse events and will act as a part of the underlying map.
        /// </summary>
        public bool Interactive { get; set; } = true;

        /// <summary>
        /// When true, a mouse event on this layer will trigger the same event on the map
        /// </summary>
        public bool BubblingMouseEvents { get; set; } = true;
    }
}
