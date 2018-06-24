import * as React from 'react';
import { Component } from 'react';

interface IHomeState {
    address: string;
    timestamp: number;
}

export class Home extends Component<{}, IHomeState> {
    public displayName = Home.name

    public constructor() {
        super({});
        this.state = {
            address: "",
            timestamp: 1529180000
        };
    }

    public render() {
        return (
            <div>
                <h1>Check ETH balance</h1>
                <p>
                    <span>
                        Address:
                    </span>
                    <input value={this.state.address} onChange={(e) => { this.setState({ address: e.target.value }); }} />
                </p>
                <p>
                    <span>
                        Timestamp:
                    </span>
                    <input value={this.state.timestamp.toString()} onChange={(e) => { this.setState({ timestamp: Number(e.target.value) }); }} />
                </p>
            </div>
        );
    }
}
