namespace Soatech.Blazor.Leaflet
{
    using Soatech.Blazor.Leaflet.Configuration;
    using Soatech.Blazor.Leaflet.Events;
    using Soatech.Blazor.Leaflet.Layers;
    using System.Drawing;

    public partial class SoaMap : IAsyncDisposable
    {
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

            NativeMap = await _jsRuntime.InvokeAsync<IJSObjectReference>("soamap.create", Id, mapOptions, DotNetObjectReference.Create(this));
        }

        #region Modifying Map State

        public ValueTask SetView(Models.LatLng coordinate, float zoom, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("setView", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoom(float zoom, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("setZoom", zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask ZoomIn(float delta, ZoomOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("zoomIn", delta, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask ZoomOut(float delta, ZoomOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("zoomOut", delta, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoomAround(Models.LatLng coordinate, float zoom, ZoomOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("setZoomAround", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetZoomAround(PointF offset, float zoom, ZoomOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("setZoomAround", offset, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FitBounds(Models.LatLng[] bounds, FitBoundsOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("fitBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FitWorld(FitBoundsOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("fitWorld", options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanTo(Models.LatLng coordinate, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("panTo", coordinate, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanBy(PointF offset, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("panBy", offset, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FlyTo(Models.LatLng coordinate, float zoom, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("flyTo", coordinate, zoom, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask FlyToBounds(Models.LatLng[] bounds, FitBoundsOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("flyToBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMaxBounds(Models.LatLng[] bounds)
        {
            return NativeMap?.InvokeVoidAsync("setMaxBounds", bounds) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMinZoom(float zoom)
        {
            return NativeMap?.InvokeVoidAsync("setMinZoom", zoom) ?? ValueTask.CompletedTask;
        }

        public ValueTask SetMaxZoom(float zoom)
        {
            return NativeMap?.InvokeVoidAsync("setMaxZoom", zoom) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanInsideBounds(Models.LatLng[] bounds, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("panInsideBounds", bounds, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask PanInside(Models.LatLng coordinate, PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("panInside", coordinate, options) ?? ValueTask.CompletedTask;
        }

        public ValueTask InvalidateSize(PanOptions options = null)
        {
            return NativeMap?.InvokeVoidAsync("invalidateSize", options) ?? ValueTask.CompletedTask;
        }

        public ValueTask InvalidateSize(bool animate)
        {
            return NativeMap?.InvokeVoidAsync("invalidateSize", animate) ?? ValueTask.CompletedTask;
        }

        public ValueTask Stop()
        {
            return NativeMap?.InvokeVoidAsync("stop") ?? ValueTask.CompletedTask;
        }

        public ValueTask AddLayer(Layer layer)
        {
            return NativeMap?.InvokeVoidAsync("addLayer", layer.NativeLayer) ?? ValueTask.CompletedTask;
        }

        #endregion

        public ValueTask<IJSObjectReference?> CreateTileLayer(TileLayerOptions config, TileLayer owner)
        {
            if (NativeMap == null) throw new InvalidOperationException("Map not initialized.");
            return NativeMap.InvokeAsync<IJSObjectReference?>("createTiles", config, DotNetObjectReference.Create(owner));
        }

        public ValueTask<IJSObjectReference?> CreateMarker(MarkerOptions options, Marker marker)
        {
            if (NativeMap == null) throw new InvalidOperationException("Map not initialized.");
            return NativeMap.InvokeAsync<IJSObjectReference?>("createMarker", options, DotNetObjectReference.Create(marker));
        }

        public ValueTask<IJSObjectReference?> CreateGroupLayer(string name, LayerGroup owner)
        {
            if (NativeMap == null) throw new InvalidOperationException("Map not initialized.");
            return NativeMap.InvokeAsync<IJSObjectReference?>("createLayerGroup", DotNetObjectReference.Create(owner));
        }

        public ValueTask HookNativeEvents()
        {
            return NativeMap?.InvokeVoidAsync("hookEvents") ?? ValueTask.CompletedTask;
        }

        private async ValueTask DisposeInteropAsync()
        {
            if (NativeMap != null)
            {
                await NativeMap.DisposeAsync();
                NativeMap = null;
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
        public ValueTask NotifyCenterChanged(ValueEvent<Models.LatLng> e)
        {
            Center = e.Value ?? new();
            return ValueTask.CompletedTask;
        }

        [JSInvokable]
        public ValueTask NotifyBoundsChanged(ValueEvent<Models.LatLng[]> e)
        {
            Bounds = new Models.LatLngBounds { SouthWest = e.Value[0], NorthEast=e.Value[1] };
            return ValueTask.CompletedTask;
        }
    }
}
