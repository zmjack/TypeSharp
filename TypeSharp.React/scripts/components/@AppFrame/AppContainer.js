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
const NavPanel_1 = __importDefault(require("./NavPanel"));
const TabsPanel_1 = __importDefault(require("./TabsPanel"));
class AppContainer extends React.Component {
    constructor(props) {
        var _a;
        super(props);
        this.state = {
            sideBar: (_a = this.props.sideBar) !== null && _a !== void 0 ? _a : true,
            navItems: props.navItems,
        };
    }
    openPane(key) {
        var pane = this.state.navItems.selectUntil(x => x.children, x => { var _a, _b; return !((_b = (_a = x === null || x === void 0 ? void 0 : x.children) === null || _a === void 0 ? void 0 : _a.any()) !== null && _b !== void 0 ? _b : false); }).firstOrDefault(x => x.key === key);
        if (pane) {
            this.tabsPanel.openPane(pane);
        }
    }
    componentDidMount() {
        var key = window.location.hash.slice(1);
        if (key != '') {
            var pane = this.state.navItems.selectUntil(x => x.children, x => { var _a, _b; return !((_b = (_a = x === null || x === void 0 ? void 0 : x.children) === null || _a === void 0 ? void 0 : _a.any()) !== null && _b !== void 0 ? _b : false); }).firstOrDefault(x => x.key === key);
            if (pane) {
                this.tabsPanel.openPane(pane);
            }
            else if (this.props.welcome != null)
                this.tabsPanel.openPane(this.props.welcome);
        }
        else if (this.props.welcome != null)
            this.tabsPanel.openPane(this.props.welcome);
    }
    render() {
        return (React.createElement("div", null,
            React.createElement("nav", { className: "navbar navbar-dark fixed-top bg-dark flex-md-nowrap p-0 shadow" },
                React.createElement("div", { className: "navbar-brand col mr-0 text-center" },
                    React.createElement("a", { href: "#", style: { color: 'white' }, onClick: () => this.setState({ sideBar: !this.state.sideBar }) }, this.props.leftRender(this))),
                React.createElement("div", { style: { color: 'white' } }, this.props.centerRender(this)),
                React.createElement("div", { style: { color: 'white' } }, this.props.rightRender(this))),
            React.createElement("div", { className: "container-fluid" },
                React.createElement("div", { className: "row" },
                    this.state.sideBar ? (React.createElement("nav", { className: "bg-light sidebar" },
                        React.createElement(NavPanel_1.default, { items: this.state.navItems, action: (key) => this.openPane(key) }))) : '',
                    React.createElement("main", { role: "main", className: 'col ml-sm-auto px-0 ' + (this.state.sideBar ? 'ml-sidebar' : 'ml-0') },
                        React.createElement(TabsPanel_1.default, { init: c => this.tabsPanel = c }))))));
    }
}
exports.default = AppContainer;
//# sourceMappingURL=AppContainer.js.map