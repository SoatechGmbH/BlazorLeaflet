namespace Soatech.Blazor.Leaflet.Models
{
    using System.Drawing;

    public class LatLng
    {
        public LatLng() { }

        public LatLng(PointF position) : this(position.X, position.Y) { }

        public LatLng(float lat, float lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public LatLng(float lat, float lng, float alt) : this(lat, lng)
        {
            Alt = alt;
        }

        public float Lat { get; set; }

        public float Lng { get; set; }

        public float Alt { get; set; }

        public PointF ToPointF() => new PointF(Lat, Lng);

        public static bool operator ==(LatLng v1, LatLng v2)
        {
            return Equals(v1, v2);
        }

        public static bool operator !=(LatLng v1, LatLng v2)
        {
            return !Equals(v1, v2);
        }

        public override bool Equals(object obj)
        {
            return obj is LatLng ll && ll.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Lat, Lng, Alt);
        }
    }
}
