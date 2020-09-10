import React, { Component } from 'react';
import { Way, Condition } from '../LiRA/Classes';

export class MapElementSelectorButton extends Component {
	constructor(props) {
		super(props);

		this.onMouseEnter = this.onMouseEnter.bind(this);
		this.onMouseLeave = this.onMouseLeave.bind(this);
		this.onClick = this.onClick.bind(this);
	}

	//Get the map-element from the specified LiRAMap instance representing this Element
	getElementLayer() {
		return this.props.element instanceof Way ? this.props.map.ways[this.props.element.id] : this.props.map.conditions[this.props.element.id]
	}

	//Internal
	//Delegate functions by firing events to the map element
	onMouseEnter() {
		this.props.map.startHover({ target: this.getElementLayer() });
	}
	onMouseLeave() {
		this.props.map.endHover({ target: this.getElementLayer() });
	}
	onClick() {
		this.onMouseLeave();
		this.props.map.select(this.props.element);
		if (this.props.goto) this.props.map.goTo(this.props.element);
	}

	render() {
		return (
			<button onMouseEnter={this.onMouseEnter} onMouseLeave={this.onMouseLeave} onClick={this.onClick}>{this.props.children}</ button>	
		)
	}
}