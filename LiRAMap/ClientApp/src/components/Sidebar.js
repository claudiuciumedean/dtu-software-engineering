import React, { Component } from 'react';
import './Sidebar.css';

class Sidebar extends Component {
    constructor(props) {
        super(props);
        this.state = { open: true }
        this.toggleSidebar = this.toggleSidebar.bind(this);
    }

    toggleSidebar() {
        this.setState({ open: !this.state.open })
    }

    render() {
        const openclosed = this.state.open ? "sidebar-open" : "sidebar-closed";
        return (
            <div className={"sidebar " + openclosed}>
                <button className={"sidebar-button " + openclosed} onClick={this.toggleSidebar}>{this.state.open ? "<" : ">"}</button>
                <div className={"sidebar-content " + openclosed}>
                    {this.props.children}
                </div>
            </div>
        );
    }
}

export { Sidebar };