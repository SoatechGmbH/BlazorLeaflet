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

    let tileConfig = removeOptionDefaults(config, [ "urlTemplate" ]);

    let tileLayer = L.tileLayer(config.urlTemplate, tileConfig);
    return tileLayer;
};

L.Map.prototype.createMarker = function (config, dotNet) {

    let markerConfig = removeOptionDefaults(config);
    let markerLayer = L.marker(config.position, markerConfig);

    hookInteractiveEvents(markerLayer, dotNet);
    hookMarkerEvents(markerLayer, dotNet);
    return markerLayer;
};

L.Map.prototype.createLayerGroup = function (name, dotNet) {

    let groupLayer = L.layerGroup();
    groupLayer.name = name;

    return groupLayer;
}

L.Map.prototype.hookEvents = function () {
    const map = this;
    const netMap = map._dotNet;

    map.on('zoomstart', function (ev) {
        let newZoomEv = { type: 'zoomstart', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('zoom', function (ev) {
        let newZoomEv = { type: 'zoom', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('zoomend', function (ev) {
        let newZoomEv = { type: 'zoomend', value: map.getZoom() };
        netMap.invokeMethodAsync('NotifyZoomChanged', newZoomEv);
    });

    map.on('movestart', function (ev) {
        let newCenter = { type: 'movestart', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('move', function (ev) {
        let newCenter = { type: 'move', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('moveend', function (ev) {
        let newCenter = { type: 'moveend', value: map.getCenter() };
        netMap.invokeMethodAsync('NotifyCenterChanged', newCenter);
    });

    map.on('contextmenu', function (ev) {
        let evt = {
            type: 'contextmenu',
            hanled: false,
            latlng: ev.latlng,
            layerPoint: ev.layerPoint,
            containerPoint: ev.containerPoint
        };
        netMap.invokeMethodAsync('NotifyContextMenu', evt);
    });
};


function hookInteractiveEvents(layer, dotNet) {
    layer.on('click', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyClickEvent", evt);
    });

    layer.on('dblclick', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyDblClickEvent", evt);
    });

    layer.on('mousedown', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyMouseDownEvent", evt);
    });

    layer.on('mouseup', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyMouseUpEvent", evt);
    });

    layer.on('mouseover', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyMouseOverEvent", evt);
    });

    layer.on('mouseout', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyMouseOutEvent", evt);
    });

    layer.on('contextmenu', function (ev) {
        let evt = cleanupEventArgsForSerialization(ev);
        dotNet.invokeMethodAsync("NotifyContextMenuEvent", evt);
    });
}

function hookMarkerEvents(layer, dotNet) {

    layer.on('movestart', function (ev) {
        let evt = { type: 'movestart', value: layer.getLatLng() };
        dotNet.invokeMethodAsync("NotifyPositionChanged", evt);
    });

    layer.on('move', function (ev) {
        let evt = { type: 'movestart', value: layer.getLatLng() };
        dotNet.invokeMethodAsync("NotifyPositionChanged", evt);
    });

    layer.on('moveend', function (ev) {
        let evt = { type: 'movestart', value: layer.getLatLng() };
        dotNet.invokeMethodAsync("NotifyPositionChanged", evt);
    });

}


function cleanupEventArgsForSerialization(eventArgs) {

    const propertiesToRemove = [
        "target",
        "sourceTarget",
        "propagatedFrom",
        "originalEvent",
        "tooltip",
        "popup"
    ];

    const copy = {};

    for (let key in eventArgs) {
        if (!propertiesToRemove.includes(key) && eventArgs.hasOwnProperty(key)) {
            copy[key] = eventArgs[key];
        }
    }

    return copy;
}

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