import * as React from "react";
import { Tabs } from 'antd'

interface Props {
    init?: (c: FrameContainer) => void;
}

interface State {
    panes: Pane[];
    activeKey: string;
}

export default class FrameContainer extends React.Component<Props, State> {

    constructor(props: Props) {
        super(props);
        this.state = {
            panes: [],
            activeKey: '',
        };
    }

    onEdit(e: React.MouseEvent | React.KeyboardEvent | string, action: 'add' | 'remove') {
        if (action == 'remove') this.remove(e as string);
    }

    onChange(activeKey: string) {
        this.setState({ activeKey: activeKey });
        window.location.href = `#${activeKey}`;
    }

    remove(targetKey: string) {
        var activeKey = this.state.activeKey;
        var lastIndex = 0;
        this.state.panes.forEach((pane, i) => {
            if (pane.key === targetKey) {
                lastIndex = i - 1;
            }
        });
        const panes = this.state.panes.filter(pane => pane.key !== targetKey);
        if (panes.length && activeKey === targetKey) {
            if (lastIndex >= 0) {
                activeKey = panes[lastIndex].key;
            } else {
                activeKey = panes[0].key;
            }
        }
        this.setState({
            panes: panes,
            activeKey: activeKey,
        });
    }

    openPane(pane: NavItem) {
        if (!this.state.panes.any(x => x.key === pane.key)) {
            var panes = this.state.panes;
            panes.push({ title: pane.title, key: pane.key, render: pane.render });
            this.setState({
                panes,
                activeKey: pane.key,
            });
        }
    }

    componentDidMount() {
        this.props.init?.(this);
    }

    render() {
        return (
            <Tabs
                type="editable-card" hideAdd
                activeKey={this.state.activeKey}
                onEdit={(e, action) => this.onEdit(e, action)}
                onChange={activeKey => this.onChange(activeKey)}>
                {
                    this.state.panes.map(pane => (
                        <Tabs.TabPane className="px-4"
                            key={pane.key}
                            tab={pane.title}
                            closable={undefined}>
                            {pane.render}
                        </Tabs.TabPane>
                    ))
                }
            </Tabs>
        );
    }
}
