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
Object.defineProperty(exports, "__esModule", { value: true });
const React = __importStar(require("react"));
const antd_1 = require("antd");
class TabsPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            panes: [],
            activeKey: '',
        };
    }
    onEdit(e, action) {
        if (action == 'remove')
            this.remove(e);
    }
    onChange(activeKey) {
        this.setState({ activeKey: activeKey });
        window.location.href = `#${activeKey}`;
    }
    remove(targetKey) {
        var activeKey = this.state.activeKey;
        var lastIndex = 0;
        this.state.panes.forEach((pane, i) => {
            if (pane.key === targetKey) {
                lastIndex = i - 1;
            }
        });
        const panes = this.state.panes.filter(pane => pane.key !== targetKey);
        if (panes.length) {
            if (lastIndex >= 0) {
                activeKey = panes[lastIndex].key;
            }
            else {
                activeKey = panes[0].key;
            }
            this.setState({
                activeKey: activeKey,
            });
            window.location.href = `#${activeKey}`;
        }
        else {
            window.location.href = `#`;
        }
        this.setState({ panes });
    }
    openPane(pane) {
        if (!this.state.panes.any(x => x.key === pane.key)) {
            var panes = this.state.panes;
            panes.push({ title: pane.title, key: pane.key, render: pane.render });
            this.setState({
                panes,
                activeKey: pane.key,
            });
        }
        else {
            this.setState({
                activeKey: pane.key,
            });
        }
        window.location.href = `#${pane.key}`;
    }
    componentDidMount() {
        var _a, _b;
        (_b = (_a = this.props).init) === null || _b === void 0 ? void 0 : _b.call(_a, this);
    }
    render() {
        return (React.createElement(antd_1.Tabs, { type: "editable-card", hideAdd: true, activeKey: this.state.activeKey, onEdit: (e, action) => this.onEdit(e, action), onChange: activeKey => this.onChange(activeKey) }, this.state.panes.map(pane => (React.createElement(antd_1.Tabs.TabPane, { className: "px-4", key: pane.key, tab: pane.title, closable: undefined }, pane.render)))));
    }
}
exports.default = TabsPanel;
//# sourceMappingURL=TabsPanel.js.map