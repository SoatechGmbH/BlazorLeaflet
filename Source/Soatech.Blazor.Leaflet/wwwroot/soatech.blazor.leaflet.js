window.soamap = {

    create: function (id, options, dotNet) {

        let mapOptions = removeOptionDefaults(options);
        mapOptions.maxBounds =
            options.maxBounds && options.maxBounds.sq && options.maxBounds.ne
                ? L.latLngBounds(options.maxBounds.sw, options.maxBounds.ne)
                : undefined;

        let map = L.map(id, mapOptions);

        map._dotNet = dotNet;

        return map;
    }
};

L.Map.prototype.createTiles = function (config, dotNet) {

    var tileConfig = removeOptionDefaults(config, [ "urlTemplate" ]);

    var tileLayer = L.tileLayer(config.urlTemplate, tileConfig)
        .addTo(this);

    tileLayer._dotNet = dotNet;

    return tileLayer;
};

L.Map.prototype.hookEvents = function () {
    const map = this;
    const netMap = map._dotNet;

    map.on('zoomstart', function (ev) {
        var newZoomEv = { type: 'zoomstart', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('zoom', function (ev) {
        var newZoomEv = { type: 'zoom', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('zoomend', function (ev) {
        var newZoomEv = { type: 'zoomend', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('movestart', function (ev) {
        var newCenter = { type: 'movestart', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('move', function (ev) {
        var newCenter = { type: 'move', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('moveend', function (ev) {
        var newCenter = { type: 'moveend', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('contextmenu', function (ev) {
        var evt = {
            type: 'contextmenu',
            hanled: false,
            latlng: ev.latlng,
            layerPoint: ev.layerPoint,
            containerPoint: ev.containerPoint
        };
        netMap.invokeMethodAsync('NotifyContextMenu', evt);
    });
};

function removeOptionDefaults(options, toRemove) {

    if (!toRemove)
        toRemove = [];

    const copy = {};

    for (let key in options) {
        if (options[key] != null && !toRemove.includes(key)) {
            copy[key] = options[key];
        }
    }

    return copy;
}