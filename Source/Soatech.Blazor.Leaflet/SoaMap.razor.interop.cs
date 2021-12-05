namespace Soatech.Blazor.Leaflet
{
    using Soatech.Blazor.Leaflet.Configuration;
    using Soatech.Blazor.Leaflet.Events;
    using Soatech.Blazor.Leaflet.Models;
    using System.Drawing;

    public partial class SoaMap : IAsyncDisposable
    {
        private IJSObjectReference? _jsSoaMap;

        private async ValueTask InitializeJsMap()
        {
            var mapOptions = new MapOptions
            {
                AttributionControl = ShowAttributionControl,
                ZoomControl = ShowZoomControl,
                MaxZoom = MaxZoom,
                MinZoom = MinZoom,
                MaxBounds = MaxBounds
            };

            _jsSoaMap = await _jsRuntime.InvokeAsync<IJSObjectReference>("soamap.create", Id, mapOptions, DotNetObjectReference.Create(this));
        }

        #region Modifying Map State

        public ValueTask SetView(LatLng coordinate, float zoom, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("setView", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoom(float zoom, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("setZoom", zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask ZoomIn(float delta, ZoomOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("zoomIn", delta, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask ZoomOut(float delta, ZoomOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("zoomOut", delta, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoomAround(LatLng coordinate, float zoom, ZoomOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("setZoomAround", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoomAround(PointF offset, float zoom, ZoomOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("setZoomAround", offset, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FitBounds(LatLng[] bounds, FitBoundsOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("fitBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FitWorld(FitBoundsOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("fitWorld", options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanTo(LatLng coordinate, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("panTo", coordinate, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanBy(PointF offset, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("panBy", offset, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FlyTo(LatLng coordinate, float zoom, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("flyTo", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FlyToBounds(LatLng[] bounds, FitBoundsOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("flyToBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMaxBounds(LatLng[] bounds)
        {
            return _jsSoaMap?.InvokeVoidAsync("setMaxBounds", bounds) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMinZoom(float zoom)
        {
            return _jsSoaMap?.InvokeVoidAsync("setMinZoom", zoom) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMaxZoom(float zoom)
        {
            return _jsSoaMap?.InvokeVoidAsync("setMaxZoom", zoom) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanInsideBounds(LatLng[] bounds, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("panInsideBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanInside(LatLng coordinate, PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("panInside", coordinate, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask InvalidateSize(PanOptions options = null)
        {
            return _jsSoaMap?.InvokeVoidAsync("invalidateSize", options) ?? ValueTask.CompletedTask;
        }

        public ValueTask InvalidateSize(bool animate)
        {
            return _jsSoaMap?.InvokeVoidAsync("invalidateSize", animate) ?? ValueTask.CompletedTask;
        }

        public ValueTask Stop()
        {
            return _jsSoaMap?.InvokeVoidAsync("stop") ?? ValueTask.CompletedTask;
        }

        #endregion

        public ValueTask<IJSObjectReference?> CreateTileLayer(TileLayerOptions config, Layers.TileLayer owner)
        {
            if (_jsSoaMap == null) throw new InvalidOperationException("Map not initialized.");
            return _jsSoaMap.InvokeAsync<IJSObjectReference?>("createTiles", config, DotNetObjectReference.Create(owner));
        }

        public ValueTask<IJSObjectReference?> CreateMarker(MarkerOptions options, Layers.Marker marker)
        {
            if (_jsSoaMap == null) throw new InvalidOperationException("Map not initialized.");
            return _jsSoaMap.InvokeAsync<IJSObjectReference?>("createMarker", options, DotNetObjectReference.Create(marker));
        }

        public ValueTask<IJSObjectReference?> CreateGroupLayer(Layers.GroupLayer owner)
        {
            if (_jsSoaMap == null) throw new InvalidOperationException("Map not initialized.");
            return _jsSoaMap.InvokeAsync<IJSObjectReference?>("createLayerGroup", DotNetObjectReference.Create(owner));
        }

        public ValueTask HookNativeEvents()
        {
            return _jsSoaMap?.InvokeVoidAsync("hookEvents") ?? ValueTask.CompletedTask;
        }

        private async ValueTask DisposeInteropAsync()
        {
            if (_jsSoaMap != null)
            {
                await _jsSoaMap.DisposeAsync();
                _jsSoaMap = null;
            }
        }

        [JSInvokable]
        public ValueTask NotifyContextMenu(MouseEvent e)
        {
            return OnContextMenu?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [JSInvokable]
        public ValueTask NotifyZoomChanged(ValueEvent<float> e)
        {
            CurrentZoom = e.Value;
            return ValueTask.CompletedTask;
        }

        [JSInvokable]
        public ValueTask NotifyCenterChanged(ValueEvent<LatLng> e)
        {
            Center = e.Value ?? new();
            return ValueTask.CompletedTask;
        }
    }
}
