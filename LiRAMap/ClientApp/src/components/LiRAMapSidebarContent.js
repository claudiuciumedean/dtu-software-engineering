import React, { PureComponent } from 'react';
import { TabbedContainer, TabbedContainerTab } from './TabbedContainer';
import { Table } from 'reactstrap';
import { LiRA } from "../LiRA/App";
import { Way, Condition } from '../LiRA/Classes';
import { MapElementSelectorButton } from './MapElementSelectorButton';


export class LiRAMapSidebarContent extends PureComponent {

    //Content for Way: Display way information, and all conditions on this way.
    renderWay() {
        var timestamp;
        if (this.props.selected.conditions.length == 0) {
            timestamp = "No conditions recorded on this way..."
        } else {
            var latest = null;
            for (var cid in this.props.selected.conditions) {
                var con = this.props.selected.conditions[cid];
                if (con.timestamp && (latest == null || con.timestamp > latest)) latest = con.timestamp;
            }

            if (latest == null) {
                timestamp = "No valid timestamps found"
            } else {
                timestamp = "Latest Condition: " + latest.toUTCString();
            }
        }

        return (
            <div>
                <div>
                    <h2>{this.props.selected.getIdentityString()}</h2>
                    <h6>(OpenStreetMap ID: {this.props.selected.id})</h6>
                    <div><i>{timestamp}</i></div>
                    <MapElementSelectorButton map={this.props.map} goto element={this.props.selected}>View on Map</MapElementSelectorButton>
                </div>

                <div>
                    <h3 className="gap-top-medium">Conditions</h3>
                    <Table className="table-light">
                        <thead>
                            <tr>
                                <th>Type</th>
                                <th>Value</th>
                                <th/>
                            </tr>
                        </thead>

                        <tbody>
                            {Object.entries(this.props.selected.conditions).map( ([_id, c]) =>
                                <tr>
                                    <td>{c.getConditionTypeName()}</td>
                                    <td>{c.translateConditionValue()}</td>
                                    <td>
                                        <MapElementSelectorButton map={this.props.map} element={c}>Select</MapElementSelectorButton>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </Table>
                </div>
            </div>
        );
    }

    //Content for Condition: Display Condition's data, and a table of all ways affected.
    renderCondition() {
        return (
            <div>
                <div>
                    <h2>{this.props.selected.getIdentityString()}</h2>
                    <div><i>{this.props.selected.timestamp.toUTCString()}</i></div>
                    <MapElementSelectorButton map={this.props.map} goto element={this.props.selected}>View on Map</MapElementSelectorButton>
                </div>

                <div className="gap-top-small">
                    <Table className="table-invisible gap-bottom-none">
                        <tbody>
                            <tr>
                                <th scope="row">Type</th>
                                <td>{this.props.selected.getConditionTypeName()}</td>
                            </tr>
                            <tr>
                                <th scope="row">{this.props.selected.getConditionTypeValueDescriptor()}</th>
                                <td>{this.props.selected.translateConditionValue()}</td>
                            </tr>
                        </tbody>
                    </Table>
                    <div>{this.props.selected.getConditionTypeDescription()}</div>

                    <h3 className="gap-top-medium gap-bottom-none">Affected Ways</h3>
                    <Table className="table-light">
                        <thead>
                            <tr>
                                <th>Way</th>
                                <th/>
                            </tr>
                        </thead>

                        <tbody>
                            {Object.keys(this.props.selected.ways).map(id =>
                                <tr>
                                    <td>{LiRA.getWay(id).getIdentityString()}</td>
                                    <td>
                                        <MapElementSelectorButton map={this.props.map} element={LiRA.getWay(id)}>Select</MapElementSelectorButton>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </Table>
                </div>
            </div>
        );
    }

    render() {
        if (this.props.selected instanceof Way) {
            return this.renderWay();
        } else if (this.props.selected instanceof Condition) {
            return this.renderCondition();
        } else {
            //Nothing selected (or at least not a Condition or Way)
            return(
                <div>
                    <h2>No selection</h2>
                    <div><i>Select an entity on the map ...</i></div>
                </div>
            )
        }
    }
}