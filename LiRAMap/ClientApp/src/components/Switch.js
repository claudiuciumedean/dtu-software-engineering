import React, { Component } from 'react';
import './Switch.css';

export class Switch extends Component {
	constructor(props) {
		super(props);
	}

	render() {
		return (
			//TODO: The Switch classes provided by the Switch.css don't work with labels. It needs to be edited. For now it's just a checkbox
			<label class={this.props.className}>
				<input type="checkbox" onChange={this.props.onChange} defaultChecked={this.props.defaultChecked}/>
				{this.props.children}
			</label>
		)
	}
}