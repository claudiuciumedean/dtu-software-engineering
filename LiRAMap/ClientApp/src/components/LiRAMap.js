import React, { Component } from 'react';
import { LeafletMap } from './LeafletMap';
import { Sidebar } from './Sidebar';
import L from 'leaflet';
import { LiRAMapSidebarContent } from './LiRAMapSidebarContent';
import { LiRA } from "../LiRA/App";
import { Way, Condition } from "../LiRA/Classes";
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Switch } from './Switch';

/*delete L.Icon.Default.prototype._getIconUrl;

L.Icon.Default.mergeOptions({
    iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
    iconUrl: require('leaflet/dist/images/marker-icon.png'),
    shadowUrl: require('leaflet/dist/images/marker-shadow.png'),
});*/

export class LiRAMap extends Component {
    constructor(props) {
        super(props);
        this.state = { selected: null };

        this.map = null;

        this.ways = {};
        this.conditions = {};
        this.wayslayer = null;
        this.conditionlayers = []; //Indexed by the type number - each condition type is on a shared layer with other conditions of that type.

        this.showways = true;
        this.showconditions = true;
        this.shownconditiontypes = [];

        var typecount = Condition.ConditionTypes.length;
        for (var i = 0; i < typecount; i++) {
            this.shownconditiontypes[i] = true;
        }
    }

    bindMap = (map) => {
        this.map = map;

        map.on("moveend", function () {
            this.procAPICall()
        }, this)

        LiRA.on("mapdataresponse", function (found) {
            found.ways.forEach(w => this.loadWay(w));
            found.conditions.forEach(c => this.loadCondition(c));
        }, this)

        //Create a layer group for the Ways associated with this map
        this.wayslayer = L.layerGroup().addTo(map);
    }

    procAPICall() {
        console.log(this);
        if (this.showways || this.showconditions) LiRA.fetchMapData(this.map.getBounds());
    }

    //Events for whenever any line or layer is hovered in the map
    //These handle calling the respective classes' Hover methods
    startHover(e) {
        e.target.setStyle({
            weight: 6
        })
    }
    endHover(e) {
        e.target.setStyle({
            weight: 3
        })
    }

    async loadWay(way) {
        if (this.ways[way.id]) {
            //Redraw it
            way.draw(this.ways[way.id]);
        } else {
            //Create a new line and hook up its events
            var line = way.draw();
            if (line != null) {
                line.addTo(this.wayslayer);

                line.on("click", e => {
                    //Select the road in the sidebar
                    this.select(way);
                    this.map.fitBounds(line.getBounds()); //View on map too
                })

                line.on("mouseover", this.startHover)
                line.on("mouseout", this.endHover)

                this.ways[way.id] = line;
            }
        }
    }

    async loadCondition(condition) {
        if (this.conditions[condition.id]) {
            //Redraw it
            condition.draw(this.conditions[condition.id]);
        } else {
            var line = condition.draw();
            if (line != null) {

                //Create a new layer group for this type if it doesn't already exist
                if (this.conditionlayers[condition.typeId] == null) this.conditionlayers[condition.typeId] = L.layerGroup().addTo(this.map);
                
                line.addTo(this.conditionlayers[condition.typeId]);
                line.on("click", e => {
                    //Select the road in the sidebar
                    this.select(condition);
                    this.map.fitBounds(line.getBounds()); //View on map too
                })

                line.on("mouseover", this.startHover)
                line.on("mouseout", this.endHover)

                this.conditions[condition.id] = line;
            }
        }
    }

    select(selection) {
        this.setState({ selected: selection });
    }

    goTo(target) {
        var layer = target instanceof Way ? this.ways[target.id] : this.conditions[target.id];
        if (layer) {
            this.map.fitBounds(layer.getBounds());
        }
    }

    loadAllKnown() {
        for (var way in LiRA.Ways) this.loadWay(LiRA.Ways[way]);
        for (var con in LiRA.Conditions) this.loadCondition(LiRA.Conditions[con]);
    }

    componentDidMount() {
        this.loadAllKnown();
    }

    toggleWays(checked) {
        this.showways = !this.showways;
        if (this.showways) {
            if (!this.map.hasLayer(this.wayslayer)) this.map.addLayer(this.wayslayer);
        } else {
            if (this.map.hasLayer(this.wayslayer)) this.map.removeLayer(this.wayslayer);
        }
    }

    toggleConditions(type, checked) {
        if (type == null) {
            this.showconditions = checked == null ? !this.showconditions : checked;
            if (this.showconditions) {

                //Only re-add those who are checked under "shownconditions"
                this.conditionlayers.forEach((layer, key) => {
                    if (this.shownconditiontypes[key] && !this.map.hasLayer(layer)) this.map.addLayer(layer);
                });
            } else {
                //Remove all that are on the map
                this.conditionlayers.forEach((layer, key) => {
                    if (this.map.hasLayer(layer)) this.map.removeLayer(layer);
                });
            }
        } else {
            this.shownconditiontypes[type] = checked == null ? !this.shownconditiontypes[type] : checked;
            if (this.showconditions) {
                var layer = this.conditionlayers[type];
                if (layer) {
                    if (this.shownconditiontypes[type]) {
                        if (!this.map.hasLayer(layer)) this.map.addLayer(layer);
                    } else {
                        if (this.map.hasLayer(layer)) this.map.removeLayer(layer);
                    }
                }
            }
        }
    }

    render() {
        //TODO: Checkboxes don't retain their checked status - re-rendered maybe?
        //Can probably be fixed by making the Switch component use a state/properly managing it's property
        return (
            <div>
                <Sidebar>
                    <LiRAMapSidebarContent selected={this.state.selected} map={this} />
                </Sidebar>
                <div className="float-top-right">
                    <UncontrolledDropdown>
                        <DropdownToggle caret>
                            Display
                        </DropdownToggle>
                        <DropdownMenu right>
                            <DropdownItem header>Global</DropdownItem>
                            <Switch defaultChecked={this.showways} className="gap-left-medium" onChange={(e) => this.toggleWays(e.target.checked)}> Ways</Switch>
                            <Switch defaultChecked={this.showconditions} className="gap-left-medium" onChange={(e) => this.toggleConditions(null, e.target.checked)}> Conditions</Switch>
                            <DropdownItem header>Condition Types</DropdownItem>

                            {this.shownconditiontypes.map((v,k) =>
                                <Switch defaultChecked={v} className="gap-left-medium" onChange={(e) => this.toggleConditions(k, e.target.checked)}>{" " + Condition.getConditionType(k).Name}</Switch>
                            )}
                        </DropdownMenu>
                    </UncontrolledDropdown>
                </div>
                <LeafletMap style={{ height: "800px", width: "100%" }} view={[55.68, 12.57]} zoom={18} bindmap={this.bindMap}/>
            </div>
        );
    }
}
