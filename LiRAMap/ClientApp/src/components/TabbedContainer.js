import React, { Component, PureComponent } from 'react';
import { Nav, NavItem, NavLink, TabContent, TabPane } from 'reactstrap';

export class TabbedContainer extends Component {
	constructor(props) {
		super(props);
		this.state = { activetab: 0 };
	}

	setTab(tab) {
		if (tab != this.state.activetab) this.setState({ activetab: tab });
	}

	render() {
		const children = this.props.children;
		const tabs = children.map((child, index) =>
			<NavItem>
				<NavLink active={index == this.state.activetab} onClick={() => this.setTab(index)}>
					{child.props.name}
				</NavLink>
			</NavItem>
		);

		const panes = children.map((child, index) =>
			<TabPane tabId={index}>
				{child}
			</TabPane>
		);

		return (
			<div>
				<Nav tabs>
					{tabs}
				</Nav>
				<TabContent activeTab={this.state.activetab}>
					{panes}
				</TabContent>
			</div>
		);
	}
}

export class TabbedContainerTab extends PureComponent {
	render() {
		return <div>this.props.children</div>;
	}
}