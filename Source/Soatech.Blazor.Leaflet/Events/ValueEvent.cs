namespace Soatech.Blazor.Leaflet.Events
{
    public class ValueEvent<T> : Event
    {
        public T? Value { get; set; }
    }
}
