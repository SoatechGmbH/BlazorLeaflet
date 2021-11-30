namespace Soatech.Blazor.Leaflet.Configuration
{
    public class PanOptions : ZoomOptions
    {
        public float Duration { get; set; } = 0.25f;
        
        public float EaseLinearity { get; set; } = 0.25f;
        
        public bool NoMoveStart { get; set; } = false;
    }
}
