import * as React from 'react';
import { Component } from 'react';

import InputMoment from "input-moment";

import 'input-moment/dist/input-moment.css';

import "../less/Home.css";

import tick from "../imgs/tick.svg";

import * as moment from "moment";

interface IHomeState {
    address: string;
    balanceData: ICheckBalanceData | null;
    m: any;
    stage: Stage;
    timestamp: number;
}

interface ICheckBalanceResponse {
    code: number;
    data: ICheckBalanceData;
}

interface ICheckBalanceData {
    Block: number;
    ETH: string;
    Tokens: ITokenBalance[];
}

interface ITokenBalance {
    Symbol: string;
    Balance: string;
}

enum Stage {
    InputStage = 1,
    Fetching = 2,
    FetchCompleted = 3,
    FetchFailed = 4
}

export class Home extends Component<{}, IHomeState> {
    public displayName = Home.name

    public constructor() {
        super({});
        this.state = {
            address: "",
            balanceData: null,
            m: moment(),
            stage: Stage.InputStage,
            timestamp: 1529180000
        };
    }

    public render() {
        return (
            <div className="centerHolder">
                <div className="title">
                    <h3>
                        Ethereum Balance Finder
                    </h3>
                </div>
                {this.renderArea()}
            </div>
        );
    }

    private renderArea() {
        switch (this.state.stage) {
            case Stage.InputStage:
                return this.renderInputStage();
            case Stage.Fetching:
                return this.renderFetchingStage();
            case Stage.FetchCompleted:
                return this.renderFetchCompletedStage();
            case Stage.FetchFailed:
                return this.renderFetchFailedStage();
            default:
                return null;
        }
    }

    private renderInputStage() {
        return (
            <div>
                <p>
                    <span>
                        Address:
                    </span>
                </p>
                <p>
                    <input className="addressInput" placeholder="Input Ethereum address" value={this.state.address} onChange={this.handleAddressChange} />
                </p>
                <InputMoment
                    moment={this.state.m}
                    onChange={this.handleTimeChange}
                    onSave={this.handleTimeSave}
                    minStep={1} // default
                    hourStep={1} // default
                    prevMonthIcon="ion-ios-arrow-left" // default
                    nextMonthIcon="ion-ios-arrow-right" // default
                />
            </div>
        );
    }

    private renderFetchingStage() {
        return (
            <div className="frameTextOnly">
                <p className="txtFetching">
                    Fetching
                </p>
            </div>
        );
    }

    private renderFetchCompletedStage() {
        return (
            <div className="contentFrame">
                <div className="resultWrapper">
                    <img className="successLogo" src={tick} width="80px" height="80px" />
                    <div className="resultFrame">
                        <p className="valueTag">
                            Block
                    </p>
                        <input disabled={true} className="resultValue" value={this.state.balanceData!.Block} />
                        <p className="valueTag">
                            ETH:
                    </p>
                        <input disabled={true} className="resultValue" value={this.state.balanceData!.ETH} />
                        {
                            this.state.balanceData!.Tokens.map((element) => <div key={element.Symbol}>
                                <p className="valueTag">
                                    {element.Symbol}:
                            </p>
                                <input disabled={true} className="resultValue" value={element.Balance} />
                            </div>)
                        }
                    </div>
                    <button
                        type="button"
                        className="backBtn"
                        onClick={this.handleBack}>
                        Back
                </button>
                </div>
            </div>
        );
    }

    private renderFetchFailedStage() {
        return (
            <div className="frameTextOnly">
                <p className="txtFailed">
                    Failed
                </p>
            </div>
        );
    }

    private handleAddressChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ address: e.target.value });
    }

    private handleTimeChange = (m: any) => {
        this.setState({ m });
    }

    private handleTimeSave = () => {

        this.setState({ stage: Stage.Fetching });

        fetch("api/balance?address=" + this.state.address + "&timestamp=" + this.state.m.unix())
            .then(response => response.json() as Promise<ICheckBalanceResponse>)
            .then(response => {
                if (response.code === 0) {
                    this.setState({
                        balanceData: response.data,
                        stage: Stage.FetchCompleted
                    });
                } else {
                    this.setState({ stage: Stage.FetchFailed });
                }
            });
    }

    private handleBack = () => {
        this.setState({ stage: Stage.InputStage });
    }
}
