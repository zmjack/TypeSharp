import * as React from "react";
import * as ReactDOM from "react-dom";
import { Linq, LinqSharp } from 'dotnet-linq'
import { Badge, Button, ConfigProvider, Popover } from 'antd';
import zhCN from 'antd/lib/locale/zh_CN';
import AppContainer from "../components/@AppFrame/AppContainer";
import { InfoCircleOutlined } from "@ant-design/icons";

Linq.enable();
LinqSharp.enable();

let params: {
    container: AppContainer,
    welcome: NavItem;
    navItems: NavItem[];
} = {
    container: null,
    welcome: {
        key: 'Welcome', title: 'Welcome',
        render: <div>
            <h1>Welcome</h1>
        </div>
    },
    navItems: [{
        key: 'Identity', title: 'Identity', children: [{
            key: 'Users', title: 'Users',
            render: <div>Users</div>,
        }, {
            key: 'Roles', title: 'Roles',
            render: <div>Roles</div>,
        }]
    }, {
        title: 'IdentityServer', key: 'IdentityServer', children: [{
            key: 'Clients', title: 'Clients',
            render: <div>IdentityServer</div>,
        }, {
            key: 'ApiResources', title: 'Api Resources',
            render: <div>ApiResources</div>,
        }, {
            key: 'ApiScopes', title: 'Api Scopes',
            render: <div>ApiScopes</div>,
        }]
    }],
}

ReactDOM.render(
    <AppContainer
        init={c => params.container = c}
        welcome={params.welcome}
        navItems={params.navItems}
        leftRender={c => <span>Identity Server</span>}
        centerRender={c => <span>Extensions</span>}
        rightRender={c => (
            <ul className="navbar-nav px-3" style={{ flexDirection: 'row' }}>
                <li className="nav-item text-nowrap ml-2">
                    <Button.Group>
                        <Badge className="notice-badge" count="5" dot={true} offset={[-10, 0]}>
                            <Button type="link" ghost>
                                <InfoCircleOutlined style={{ color: 'white' }} />
                            </Button>
                        </Badge>
                        <Badge className="notice-badge" count="5" dot={true} offset={[-10, 0]}>
                            <Button type="link" ghost>
                                <InfoCircleOutlined style={{ color: 'white' }} />
                            </Button>
                        </Badge>
                    </Button.Group>
                </li>
                <li className="nav-item text-nowrap ml-2 mr-1">
                    <Popover title="User" trigger="click" placement="bottomRight"
                        content={
                            <div>
                                <Button type="link" block
                                    onClick={() => window.location.href = '#'}>
                                    Sign out
                                </Button>
                            </div>
                        }>
                        <div style={{ paddingLeft: 12 }}>
                            <a className="nav-link" href="#">User</a>
                        </div>
                    </Popover>
                </li>
            </ul>
        )} />,
    document.getElementById("app")
);
