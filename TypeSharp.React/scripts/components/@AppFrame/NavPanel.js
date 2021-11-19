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
class NavPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            expandedKeys: [],
            searchValue: '',
            autoExpandParent: true,
            flatKeys: [],
        };
    }
    initFlatItems(items) {
        this.setState({
            flatKeys: this.props.items.selectMore(x => x.children).select(x => x.key),
        });
    }
    ;
    getParentKey(key, tree = this.props.items) {
        let parentKey;
        for (let i = 0; i < tree.length; i++) {
            const node = tree[i];
            if (node.children) {
                if (node.children.some(item => item.key === key)) {
                    parentKey = node.key;
                }
                else if (this.getParentKey(key, node.children)) {
                    parentKey = this.getParentKey(key, node.children);
                }
            }
        }
        return parentKey;
    }
    ;
    toggleKey(key) {
        var expandedKeys = this.state.expandedKeys;
        var index = this.state.expandedKeys.indexOf(key);
        if (index > -1)
            expandedKeys.splice(index, 1);
        else
            expandedKeys.push(key);
        this.setState({
            expandedKeys: [...expandedKeys],
            autoExpandParent: false,
        });
    }
    onExpand(expandedKeys) {
        this.setState({
            expandedKeys,
            autoExpandParent: false,
        });
    }
    onChange(e) {
        const value = e.target.value;
        let expandedKeys;
        if (value !== '') {
            expandedKeys = this.state.flatKeys
                .map(key => {
                if (key.indexOf(value) > -1)
                    return this.getParentKey(key);
                else
                    return null;
            })
                .filter((item, i, self) => item && self.indexOf(item) === i);
        }
        else
            expandedKeys = [];
        this.setState({
            expandedKeys,
            searchValue: value,
            autoExpandParent: true,
        });
    }
    componentDidMount() {
        var _a, _b;
        (_b = (_a = this.props).init) === null || _b === void 0 ? void 0 : _b.call(_a, this);
        this.initFlatItems(this.props.items);
    }
    render() {
        var title_area = (item) => {
            var key = item.key;
            var title = item.title;
            var content = title.indexOf(this.state.searchValue) > -1 ? (React.createElement("span", null,
                title.substr(0, title.indexOf(this.state.searchValue)),
                React.createElement("span", { style: { color: '#f50' } }, this.state.searchValue),
                title.substr(title.indexOf(this.state.searchValue) + this.state.searchValue.length))) : (React.createElement("span", null, title));
            if (item.render != null) {
                return React.createElement("span", { onClick: () => this.props.action(key) }, content);
            }
            else {
                return React.createElement("span", { onClick: () => this.toggleKey(key) }, content);
            }
        };
        return (React.createElement("div", null,
            React.createElement(antd_1.Input.Search, { size: "large", placeholder: "Search", onChange: e => this.onChange(e) }),
            React.createElement(antd_1.Tree, { style: { marginTop: 16 }, showIcon: true, onExpand: expandedKeys => this.onExpand(expandedKeys), expandedKeys: this.state.expandedKeys, autoExpandParent: this.state.autoExpandParent, treeData: this.props.items, titleRender: (node) => title_area(node) })));
    }
}
exports.default = NavPanel;
//# sourceMappingURL=NavPanel.js.map