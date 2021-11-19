import * as React from "react";
import * as ReactDOM from "react-dom";
import { Linq } from 'dotnet-linq'
import { Badge, Button, ConfigProvider, Popover } from 'antd';
import zhCN from 'antd/lib/locale/zh_CN';
import FrameContainer from "../components/@Frame/FrameContainer";
import { InfoCircleOutlined } from "@ant-design/icons";

Linq.enable();
let params: {
    tabsFrame: FrameContainer,
    navItems: NavItem[];
} = {
    tabsFrame: null,
    navItems: [{
        key: 'Identity', title: 'Identity', scopedSlots: { title: 'title' }, children: [{
            key: 'Users', title: 'Users', scopedSlots: { title: 'title', icon: 'icon_smile' },
            render: h => <div>Users</div>,
        }, {
            key: 'Roles', title: 'Roles', scopedSlots: { title: 'title', icon: 'icon_smile' },
            render: h => <div>Roles</div>,
        }]
    }, {
        title: 'IdentityServer', key: 'IdentityServer', scopedSlots: { title: 'title' }, children: [{
            key: 'Clients', title: 'Clients', scopedSlots: { title: 'title', icon: 'icon_smile' },
            render: h => <div>IdentityServer</div>,
        }, {
            key: 'ApiResources', title: 'Api Resources', scopedSlots: { title: 'title', icon: 'icon_smile' },
            render: h => <div>ApiResources</div>,
        }, {
            key: 'ApiScopes', title: 'Api Scopes', scopedSlots: { title: 'title', icon: 'icon_smile' },
            render: h => <div>ApiScopes</div>,
        }]
    }],
}

ReactDOM.render(
    <FrameContainer
        nav-items={params.navItems}>

        <span slot="left">Identity Server</span>
        <div slot="center">Extensions22</div>
        <ul slot="right" className="navbar-nav px-3" style={{ flexDirection: 'row' }}>
            <li className="nav-item text-nowrap ml-2">
                <Button.Group>
                    <Badge className="notice-badge" count="5" dot={true} offset={[-10, 0]}>
                        <Button type="link" ghost>
                            <InfoCircleOutlined />
                        </Button>
                    </Badge>
                    <Badge className="notice-badge" count="5" dot={true} offset={[-10, 0]}>
                        <Button type="link" ghost>
                            <InfoCircleOutlined />
                        </Button>
                    </Badge>
                </Button.Group>
            </li>
            <li className="nav-item text-nowrap ml-3 mr-1">
                <Popover title="User" trigger="click" placement="bottomRight">
                    <div slot="content">
                        <Button type="link" block
                            onClick={() => window.location.href = '#'}>
                            Sign out
                        </Button>
                    </div>
                    <a className="nav-link" href="#">User</a>
                </Popover>
            </li>
        </ul>
    </FrameContainer>,
    document.getElementById("app")
);
