using Microsoft.JSInterop;
using Soatech.Blazor.Leaflet.Configuration;
using Soatech.Blazor.Leaflet.Events;
using Soatech.Blazor.Leaflet.Models;
using System.Reactive;
using System.Reactive.Linq;

namespace Soatech.Blazor.Leaflet
{
    public partial class SoaMap : IAsyncDisposable
    {
        private LatLng _contextCoordinates;
        private IJSObjectReference? _jsSoaMap;

        private async ValueTask InitializeJsMap()
        {
            var mapOptions = new MapOptions
            {
                AttributionControl = ShowAttributionControl,
                ZoomControl = ShowZoomControl
            };

            _jsSoaMap = await _jsRuntime.InvokeAsync<IJSObjectReference>("soamap.create", _id, mapOptions, DotNetObjectReference.Create(this));
        }

        public IObservable<Unit> SetView(LatLng coordinate, float zoom)
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("setView", coordinate, zoom).AsTask() ?? Task.CompletedTask);
        }

        public IObservable<Unit> SetZoom(float zoom)
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("setZoom", zoom).AsTask() ?? Task.CompletedTask);
        }

        public IObservable<Unit> PanTo(LatLng coordinate)
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("panTo", coordinate).AsTask() ?? Task.CompletedTask);
        }

        public IObservable<Unit> FlyTo(LatLng coordinate, float zoom)
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("flyTo", coordinate, zoom).AsTask() ?? Task.CompletedTask);
        }

        public IObservable<Unit> HookEvents()
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("hookEvents").AsTask() ?? Task.CompletedTask);
        }

        public IObservable<Unit> FitWorld()
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("fitWorld").AsTask() ?? Task.CompletedTask);
        }

        public async ValueTask DisposeAsync()
        {
            if (_jsSoaMap != null)
            {
                await _jsSoaMap.DisposeAsync();
                _jsSoaMap = null;
            }
            GC.SuppressFinalize(this);
        }

        private IObservable<Unit> CreateTileLayer(TileLayerConfiguration config)
        {
            return Observable.FromAsync(() => _jsSoaMap?.InvokeVoidAsync("createTiles", config).AsTask() ?? Task.CompletedTask);
        }

        [JSInvokableAttribute]
        public ValueTask NotifyContextMenu(MouseEvent e)
        {
            _contextCoordinates = e.LatLng;
            return ValueTask.CompletedTask;
        }

        [JSInvokableAttribute]
        public ValueTask NotifyZoomChanged(ValueEvent<float> e)
        {
            CurrentZoom = e.Value;
            return ValueTask.CompletedTask;
        }

        [JSInvokableAttribute]
        public ValueTask NotifyCenterChanged(ValueEvent<LatLng> e)
        {
            Center = e.Value ?? new();
            return ValueTask.CompletedTask;
        }
    }
}
