namespace Soatech.Blazor.Leaflet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LatLngBounds
    {
        public LatLng SouthWest { get; set; } = new LatLng(0, 0);
        public LatLng NorthEast { get; set; } = new LatLng(0, 0);

        public LatLngBounds() { }

        public LatLngBounds(LatLng southwestcorner, LatLng northeastcorner)
        {
            SouthWest = southwestcorner;
            NorthEast = northeastcorner;
        }
    }
}
