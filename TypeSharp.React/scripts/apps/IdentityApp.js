"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const React = __importStar(require("react"));
const ReactDOM = __importStar(require("react-dom"));
const dotnet_linq_1 = require("dotnet-linq");
const antd_1 = require("antd");
const AppContainer_1 = __importDefault(require("../components/@AppFrame/AppContainer"));
const icons_1 = require("@ant-design/icons");
dotnet_linq_1.Linq.enable();
dotnet_linq_1.LinqSharp.enable();
let params = {
    container: null,
    welcome: {
        key: 'Welcome', title: 'Welcome',
        render: React.createElement("div", null,
            React.createElement("h1", null, "Welcome"))
    },
    navItems: [{
            key: 'Identity', title: 'Identity', children: [{
                    key: 'Users', title: 'Users',
                    render: React.createElement("div", null, "Users"),
                }, {
                    key: 'Roles', title: 'Roles',
                    render: React.createElement("div", null, "Roles"),
                }]
        }, {
            title: 'IdentityServer', key: 'IdentityServer', children: [{
                    key: 'Clients', title: 'Clients',
                    render: React.createElement("div", null, "IdentityServer"),
                }, {
                    key: 'ApiResources', title: 'Api Resources',
                    render: React.createElement("div", null, "ApiResources"),
                }, {
                    key: 'ApiScopes', title: 'Api Scopes',
                    render: React.createElement("div", null, "ApiScopes"),
                }]
        }],
};
ReactDOM.render(React.createElement(AppContainer_1.default, { init: c => params.container = c, welcome: params.welcome, navItems: params.navItems, leftRender: c => React.createElement("span", null, "Identity Server"), centerRender: c => React.createElement("span", null, "Extensions"), rightRender: c => (React.createElement("ul", { className: "navbar-nav px-3", style: { flexDirection: 'row' } },
        React.createElement("li", { className: "nav-item text-nowrap ml-2" },
            React.createElement(antd_1.Button.Group, null,
                React.createElement(antd_1.Badge, { className: "notice-badge", count: "5", dot: true, offset: [-10, 0] },
                    React.createElement(antd_1.Button, { type: "link", ghost: true },
                        React.createElement(icons_1.InfoCircleOutlined, { style: { color: 'white' } }))),
                React.createElement(antd_1.Badge, { className: "notice-badge", count: "5", dot: true, offset: [-10, 0] },
                    React.createElement(antd_1.Button, { type: "link", ghost: true },
                        React.createElement(icons_1.InfoCircleOutlined, { style: { color: 'white' } }))))),
        React.createElement("li", { className: "nav-item text-nowrap ml-2 mr-1" },
            React.createElement(antd_1.Popover, { title: "User", trigger: "click", placement: "bottomRight", content: React.createElement("div", null,
                    React.createElement(antd_1.Button, { type: "link", block: true, onClick: () => window.location.href = '#' }, "Sign out")) },
                React.createElement("div", { style: { paddingLeft: 12 } },
                    React.createElement("a", { className: "nav-link", href: "#" }, "User")))))) }), document.getElementById("app"));
//# sourceMappingURL=IdentityApp.js.map