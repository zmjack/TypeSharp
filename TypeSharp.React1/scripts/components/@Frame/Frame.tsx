import * as React from "react";
import { MenuOutlined } from "@ant-design/icons";
import NavPanel from './NavPanel'
import FrameContainer from './FrameContainer'

interface Props {
    init?: (c: Frame) => void;
    navItems: NavItem[];

    leftRender: (c: Frame) => JSX.Element;
    centerRender: (c: Frame) => JSX.Element;
    rightRender: (c: Frame) => JSX.Element;

    sideBar?: boolean;
}

interface State {
    sideBar: boolean;
    navItems: NavItem[];
}

export default class Frame extends React.Component<Props, State> {

    private mainTabsPanel: FrameContainer;

    constructor(props: Props) {
        super(props);
        this.state = {
            sideBar: this.props.sideBar ?? true,
            navItems: props.navItems,
        }
    }

    openPane(key: string) {
        var pane = this.state.navItems.selectUntil(x => x.children, x => !(x?.children.any() ?? false)).firstOrDefault(x => x.key === key);
        if (pane) {
            this.mainTabsPanel.openPane(pane);
        }
    }

    componentDidMount() {
        var key = window.location.hash.slice(1);
        this.openPane(key);
    }

    render() {
        return (
            <div>
                <nav className="navbar navbar-dark fixed-top bg-dark flex-md-nowrap p-0 shadow">
                    <div className="navbar-brand col mr-0">
                        <MenuOutlined style={{ cursor: 'cursor: pointer' }} onClick={() => this.setState({ sideBar: !this.state.sideBar })} />
                        <a href="#" style={{ color: 'white' }}>
                            {
                                this.props.leftRender(this)
                            }
                        </a>
                    </div>
                    <div style={{ color: 'white' }}>
                        {
                            this.props.centerRender(this)
                        }
                    </div>
                    <div style={{ color: 'white' }}>
                        {
                            this.props.rightRender(this)
                        }
                    </div>
                </nav>

                <div className="container-fluid">
                    <div className="row">
                        {
                            this.state.sideBar ? (
                                <nav className="bg-light sidebar">
                                    <NavPanel
                                        items={this.state.navItems}
                                        action={(key: string) => this.openPane(key)}>
                                    </NavPanel>
                                </nav>
                            ) : ''
                        }
                        <main role="main" className={'col ml-sm-auto px-0 ' + (this.state.sideBar ? 'ml-sidebar' : 'ml-0')}>
                            <FrameContainer init={c => this.mainTabsPanel = c}></FrameContainer>
                        </main>
                    </div>
                </div>
            </div>
        );
    }
}
