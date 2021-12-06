namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    using ReactiveUI;
    using Soatech.Blazor.Leaflet.Models;

    public class LatLngViewModel : ReactiveObject
    {
        private LatLng _value;

        public LatLngViewModel(LatLng position)
        {
            _value = position;
        }

        public LatLng Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(Lat));
                this.RaisePropertyChanged(nameof(Lng));
            }
        }

        public float Lat
        {
            get => _value.Lat;
            set
            {
                if (_value.Lat == value) return;

                _value = new LatLng(value, _value.Lng);
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(Value));
            }
        }

        public float Lng
        {
            get => _value.Lng;
            set
            {
                if (_value.Lng == value) return;

                _value = new LatLng(_value.Lat, value);
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
