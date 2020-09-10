import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { LiRAMap } from './components/LiRAMap';
import { FetchData } from './components/FetchData';
import { LiRAList } from './components/LiRAList';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={LiRAMap} />
        <Route path='/list' component={LiRAList} />
      </Layout>
    );
  }
}
