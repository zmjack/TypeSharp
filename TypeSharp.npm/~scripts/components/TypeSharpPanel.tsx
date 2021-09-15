import * as React from "react";
import { ApiHelper } from "../npm-clone";
import { HomeApi } from './type-sharp-demo.api';

export interface TypeSharpPanelProps {
}

export interface TypeSharpPanelState {
    content: string,
}

export class TypeSharpPanel extends React.Component<TypeSharpPanelProps, TypeSharpPanelState> {

    private api = new HomeApi(new ApiHelper({
        beforeResolve: response => {
            var data = response.data;
            if (data?.toString() == '[object Blob]')
                this.setState({ content: `[Download file]` });
            else this.setState({ content: `${data.toString()} ${data}` });
        },
        beforeReject: reason => {
            this.setState({ content: `${reason}` });
        },
    }));

    constructor(props: TypeSharpPanelProps) {
        super(props);
        this.state = {
            content: '<null>'
        };
    }

    render() {
        return <div>
            <div>
                <button type="button" onClick={() => this.api.getContent().then(content => this.setState({ content }))}>GetContent</button>
                <button type="button" onClick={() => this.api.getContent500()}>GetContent500</button>

                <button type="button" onClick={() => this.api.getFile()}>GetFile</button>
                <button type="button" onClick={() => this.api.getFile404()}>GetFile404</button>
                <button type="button" onClick={() => this.api.getFile500()}>GetFile500</button>
            </div>
            <div>
                content: {this.state.content}
            </div>
        </div>
    }
}
