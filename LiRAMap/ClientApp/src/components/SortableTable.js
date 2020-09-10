import React, { Component, PureComponent } from 'react';
import { Table } from 'reactstrap';

export class SortableTable extends Component {
	constructor(props) {
		super(props);
		this.state = { sortedcolumn: 0 };
	}

	setSortedColumn(column) {
		if (column != this.state.sortedcolumn && column > 0 && column < this.props.children[0].props.children.length) this.setState({ sortedcolumn: column });
	}

	render() {
		var body = this.props.children[1].props.children.concat();
		var comparator = this.props.children[0].props.children[this.state.sortedcolumn].props.comparator;
		if (!comparator) {
			comparator = ((a, b) => {
				return a > b ? 1 : a < b ? -1 : 0;
			});
		}

		if (body) body.sort((a, b) => {
			var column = this.state.sortedcolumn;
			var x = a.props.children[column].props.value;
			var y = b.props.children[column].props.value;

			console.log(column, x, y);
			console.log(a);
			console.log(b);

			return comparator(x, b);
		});

		console.log(body);

		return (
			<Table>
				<thead>
					{this.props.children[0].props.children.map((child, index) =>
						<th onClick={() => this.setState({ sortedcolumn: index })}>
							{child}
						</th>
					)}
				</thead>
				<tbody>
					{body}
				</tbody>
			</Table>
		);
	}
}

export class SortableTableColumn extends PureComponent {
	render() {
		return this.props.children;
	}
}

export class SortableTableCell extends PureComponent {
	render() {
		return (<div>{this.props.children}</div>);
	}
}