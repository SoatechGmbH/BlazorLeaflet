window.soamap = function () {

    const my = this;

    this._id = "";
    this._leaflet = null;
    this._tileLayer = null;
    this._dotNet = null;

    this.createTiles = function (config) {
        const replace = my._tileLayer !== null;

        let center = undefined;
        let zoom = undefined;

        if (replace) {
            zoom = my.getZoom();
            center = my.getCenter();
            my._tileLayer.remove(my._leaflet);
        }

        my._tileLayer = L.tileLayer(config.urlTemplate, {
            id: config.id,
            attribution: config.attribution,
            minZoom: config.minZoom,
            maxZoom: config.maxZoom,
            tileSize: config.tileSize,
            zoomOffset: config.zoomOffset
        }).addTo(my._leaflet);

        if (replace) {
            my.setView(center, zoom);
        }
    };

    this.fitWorld = function () {
        my._leaflet.fitWorld();
    };

    this.setView = function (coord, zoom) {
        my._leaflet.setView(coord, zoom);
    };
    this.setZoom = function (zoom) {
        my._leaflet.setZoom(zoom);
    };
    this.panTo = function (coord) {
        my._leaflet.panTo(coord);
    };
    this.flyTo = function (coord, zoom) {
        my._leaflet.flyTo(coord, zoom);
    };

    this.getCenter = function () {
        return my._leaflet.getCenter();
    };
    this.getZoom = function () {
        return my._leaflet.getZoom();
    };
    this.getBounds = function () {
        return my._leaflet.getBounds();
    };

    this.hookEvents = function () {
        const map = my._leaflet;
        const netMap = my._dotNet;

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
};

window.soamap.create = function (id, options, dotNet) {
    let map = new soamap();
    map._id = id;
    map._dotNet = dotNet;
    map._leaflet = L.map(id, options);

    return map;
}