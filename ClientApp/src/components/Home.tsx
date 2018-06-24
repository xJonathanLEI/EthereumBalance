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
                    <input value={this.state.address} onChange={this.handleAddressChange} />
                </p>
                <p>
                    <span>
                        Timestamp:
                    </span>
                    <input value={this.state.timestamp.toString()} onChange={this.handleTimestampChange} />
                </p>
            </div>
        );
    }

    private handleAddressChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ address: e.target.value });
    }

    private handleTimestampChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ timestamp: Number(e.target.value) });
    }
}
