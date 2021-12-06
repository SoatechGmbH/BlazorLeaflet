using Soatech.Blazor.Leaflet.Samples.ViewModels;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Soatech.Blazor.Leaflet.Samples.Pages
{
    public partial class SoaMapDemo
    {
        private SoaMap? _mapControl;
        private SerialDisposable serialDisposable = new SerialDisposable();

        protected override Task OnInitializedAsync()
        {
            serialDisposable.Disposable = 
                Observable.Timer(TimeSpan.FromSeconds(5))
                .SelectMany(_ => 
                    _mapControl?.FlyTo(new() { Lat = 53.57532f, Lng = 10.01534f }, 6)
                        .AsTask()
                        .ToObservable() 
                    ?? Observable.Empty<Unit>())
                .Subscribe(_ =>
                {
                    serialDisposable.Disposable =
                        Observable.Timer(TimeSpan.FromSeconds(5))
                        .Subscribe(_ =>
                        {
                            ViewModel.TileLayer = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
                            ViewModel.MarkerPosition.Value = new() { Lat = 53.57532f, Lng = 10.01534f };
                            //ViewModel.Center = new();
                            //ViewModel.MinZoom = 8;
                            //ViewModel.MaxZoom = 7;

                            serialDisposable.Disposable =
                                Observable.Timer(TimeSpan.FromSeconds(20))
                                .Subscribe(_ =>
                                {
                                    if (_mapControl.MapLayers.Count > 1)
                                        _mapControl.MapLayers.RemoveAt(1);
                                });
                        });
                });

            return base.OnInitializedAsync();
        }

        private ValueTask OnMarkerClicked(MarkerViewModel viewModel)
        {
            ViewModel.SelectMarker(viewModel);
            StateHasChanged();
            return ValueTask.CompletedTask;
        }
    }
}
