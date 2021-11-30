using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Soatech.Blazor.Leaflet.Samples.Pages
{
    public partial class SoaMapDemo
    {
        private SoaMap? _mapControl;

        protected override Task OnInitializedAsync()
        {
            Observable.Timer(TimeSpan.FromSeconds(10))
                .SelectMany(_ => 
                    _mapControl?.FlyTo(new() { Lat = 53.57532f, Lng = 10.01534f }, 6)
                        .AsTask()
                        .ToObservable() 
                    ?? Observable.Empty<Unit>())
                .Subscribe();

            Observable.Timer(TimeSpan.FromSeconds(20))
                .Subscribe(_ =>
                {
                    ViewModel.TileLayer = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
                    //ViewModel.Center = new();
                    //ViewModel.MinZoom = 8;
                    //ViewModel.MaxZoom = 7;
                });

            //Observable.Timer(TimeSpan.FromSeconds(30))
            //    .Subscribe(mc =>
            //    {
            //        ViewModel.TileLayer = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
            //    });

            return base.OnInitializedAsync();
        }
    }
}
