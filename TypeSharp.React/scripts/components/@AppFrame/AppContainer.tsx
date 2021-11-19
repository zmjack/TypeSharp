import * as React from "react";
import { MenuOutlined } from "@ant-design/icons";
import NavPanel from './NavPanel'
import TabsPanel from './TabsPanel'

interface Props {
    init?: (c: AppContainer) => void;
    navItems: NavItem[];

    leftRender: (c: AppContainer) => JSX.Element;
    centerRender: (c: AppContainer) => JSX.Element;
    rightRender: (c: AppContainer) => JSX.Element;

    sideBar?: boolean;
    welcome?: NavItem;
}

interface State {
    sideBar: boolean;
    navItems: NavItem[];
}

export default class AppContainer extends React.Component<Props, State> {

    private tabsPanel: TabsPanel;

    constructor(props: Props) {
        super(props);
        this.state = {
            sideBar: this.props.sideBar ?? true,
            navItems: props.navItems,
        }
    }

    openPane(key: string) {
        var pane = this.state.navItems.selectUntil(x => x.children, x => !(x?.children?.any() ?? false)).firstOrDefault(x => x.key === key);
        if (pane) {
            this.tabsPanel.openPane(pane);
        }
    }

    componentDidMount() {
        var key = window.location.hash.slice(1);
        if (key != '') {
            var pane = this.state.navItems.selectUntil(x => x.children, x => !(x?.children?.any() ?? false)).firstOrDefault(x => x.key === key);
            if (pane) {
                this.tabsPanel.openPane(pane);
            }
            else if (this.props.welcome != null) this.tabsPanel.openPane(this.props.welcome);
        }
        else if (this.props.welcome != null) this.tabsPanel.openPane(this.props.welcome);
    }

    render() {
        return (
            <div>
                <nav className="navbar navbar-dark fixed-top bg-dark flex-md-nowrap p-0 shadow">
                    <div className="navbar-brand col mr-0 text-center">
                        <a href="#" style={{ color: 'white' }}
                            onClick={() => this.setState({ sideBar: !this.state.sideBar })}>
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
                            <TabsPanel init={c => this.tabsPanel = c}></TabsPanel>
                        </main>
                    </div>
                </div>
            </div>
        );
    }
}
