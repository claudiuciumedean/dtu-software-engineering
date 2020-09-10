import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { LiRA } from '../LiRA/App';
import { LiRASearchBar } from './LiRASearchBar';

export class NavMenu extends Component {
static displayName = NavMenu.name;

    constructor (props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
            api: false
        };

        LiRA.on("apistart", () => this.setState({ api: true }));
        LiRA.on("apiend", () => this.setState({ api: false }));
    }

    toggleNavbar () {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    render () {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">LiRAMap</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <LiRASearchBar />
              
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <div>{this.state.api ? "Fetching data..." : ""}</div>
                                </NavItem>
                
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">Map</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/list">List</NavLink>
                                </NavItem>
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
