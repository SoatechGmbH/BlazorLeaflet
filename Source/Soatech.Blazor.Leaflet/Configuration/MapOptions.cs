namespace Soatech.Blazor.Leaflet.Configuration
{
    using Soatech.Blazor.Leaflet.Models;

    public class MapOptions
    {
        public bool PreferCanvas { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whetehr to show the attribution control.
        /// </summary>
        public bool AttributionControl { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whetehr to show the zoom control.
        /// </summary>
        public bool ZoomControl { get; set; } = true;

        public bool ClosePopupOnClick { get; set; } = false;
        public float ZoomSnap { get; set; } = 1;
        public float ZoomDelta { get; set; } = 1;
        public bool TrackResize { get; set; } = true;
        public bool BoxZoom { get; set; } = true;
        public bool DoubleClickZoom { get; set; } = true;
        public bool Dragging { get; set; } = true;

        public float? Zoom { get; set; }

        /// <summary>
        /// Gets or sets the minimum zoom level of the map. If not specified and at least one 
        /// GridLayer or TileLayer is in the map, the lowest of their minZoom
        /// options will be used instead.
        /// </summary>
        public float? MinZoom { get; set; }

        /// <summary>
        /// Gets or sets the maximum zoom level of the map. If not specified and at least one
        /// GridLayer or TileLayer is in the map, the highest of their maxZoom
        /// options will be used instead.
        /// </summary>
        public float? MaxZoom { get; set; }

        public Layer[] Layers { get; set; } = new Layer[0];

        /// <summary>
        /// Gets or sets the bounding restrictions for the map.
        /// When this option is set, the map restricts the view to the given
        /// geographical bounds, bouncing the user back if the user tries to pan
        /// outside the view.
        /// </summary>
        public LatLngBounds? MaxBounds { get; set; }

        public bool ZoomAnimation { get; set; } = true;
        public float ZoomAnimationThreshold { get; set; } = 4;
        public bool FadeAnimation { get; set; } = true;
        public bool MarkerZoomAnimation { get; set; } = true;

        public bool? Inertia { get; set; }
        public float InertiaDeceleration { get; set; } = 3000;
        public float? InertiaMaxSpeed { get; set; }
        public float EaseLinearity { get; set; } = 0.2f;
        public bool WorldCopyJump { get; set; } = false;
        public float MaxBoundsViscosity { get; set; } = 0;

        public bool Keyboard { get; set; } = true;
        public float KeyboardPanDelta { get; set; } = 80;

        public object ScrollWheelZoom { get; set; } = true;
        public float WheelDebounceTime { get; set; } = 40;
        public float WheelPxPerZoomLevel { get; set; } = 60;

        public bool Tap { get; set; } = true;
        public float TapTolerance { get; set; } = 15;
        public object TouchZoom { get; set; } = null;
        public bool BounceAtZoomLimits { get; set; } = true;
    }
}
