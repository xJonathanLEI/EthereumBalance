import * as React from 'react';
import { Component } from 'react';
import { Route } from 'react-router';
import { Home } from './components/Home';

export default class App extends Component {
    public displayName = App.name

    public render() {
        return (
            <Route exact={true} path='/' component={Home} />
        );
    }
}
