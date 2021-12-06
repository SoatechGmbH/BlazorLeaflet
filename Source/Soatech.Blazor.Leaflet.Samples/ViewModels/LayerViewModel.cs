namespace Soatech.Blazor.Leaflet.Samples.ViewModels
{
    using ReactiveUI;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public class LayerViewModel : ReactiveObject, IAsyncDisposable
    {
        private string _name = "";
        private bool _isVisible = true;
        private ObservableCollection<MarkerViewModel> _markers = new();

        public string Name 
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public ObservableCollection<MarkerViewModel> Markers
        {
            get => _markers;
            set => this.RaiseAndSetIfChanged(ref _markers, value);
        }

        public void AddMarker(MarkerViewModel marker)
        {
            Markers.Add(marker);
            marker.PropertyChanged += (o, e ) => this.RaisePropertyChanged(nameof(Markers));
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
