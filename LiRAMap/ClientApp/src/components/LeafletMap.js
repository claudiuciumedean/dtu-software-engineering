import React, { Component } from 'react';
import 'leaflet/dist/leaflet.css'
import L from 'leaflet';

export class LeafletMap extends Component {
    constructor(props) {
        super(props);

        this.map = null;
        this.container = null;
    }

    initializeMap = div => {
        this.container = div;
    }

    static style = {
        width: "100%",
        height: "100%",
        zIndex: 0
    }
    static mapSource = {
        url: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }

    componentDidMount() {
        if (this.map == null && this.container != null) {
            this.map = L.map(this.container, {
                center: this.props.view,
                zoom: this.props.zoom
            })

            var layer = L.tileLayer(LeafletMap.mapSource.url, {
                attribution: LeafletMap.mapSource.attribution
            });
            this.map.addLayer(layer);

            if (this.props.bindmap != null) this.props.bindmap(this.map, layer);
        }
    }

    render() {
        return (
            <div style={this.props.style}>
                <div ref={this.initializeMap} style={LeafletMap.style} />
            </div>
        );
    }
}
