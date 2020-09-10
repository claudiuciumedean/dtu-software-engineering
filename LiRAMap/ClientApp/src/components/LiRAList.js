import React, { Component } from 'react';
import { SortableTable, SortableTableColumn, SortableTableCell } from './SortableTable';
import './LiRAList.css';

export class LiRAList extends Component {
  

    constructor(props) {
        super(props);

    }

    render() {
        return (
            <div>
                <button className="create-condition">New Condition</button>
                <SortableTable>
                    <thead>
                        <SortableTableColumn>#</SortableTableColumn>
                        <SortableTableColumn>Way</SortableTableColumn>
                    </thead>
                    <tbody>
                        <tr>
                            <SortableTableCell value={1}>1</SortableTableCell>
                            <SortableTableCell value={12341231}>12341231</SortableTableCell>
                        </tr>
                        <tr>
                            <SortableTableCell value={2}>2</SortableTableCell>
                            <SortableTableCell value={41234412}>41234412</SortableTableCell>
                        </tr>
                        <tr>
                            <SortableTableCell value={3}>3</SortableTableCell>
                            <SortableTableCell value={23897413}>23897413</SortableTableCell>
                        </tr>
                    </tbody>
                </SortableTable>
            </div>
        );
    }
}
