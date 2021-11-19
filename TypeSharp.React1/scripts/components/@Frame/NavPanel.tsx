import * as React from "react";
import { Input, Tabs, Tree } from 'antd'

interface Props {
    init?: (c: NavPanel) => void;
    items: NavItem[];
    action: (key: string) => void;
}

interface State {
    expandedKeys: string[];
    searchValue: string;
    autoExpandParent: boolean;
    flatKeys: string[];
}

export default class NavPanel extends React.Component<Props, State> {

    constructor(props: Props) {
        super(props);
        this.state = {
            expandedKeys: [],
            searchValue: '',
            autoExpandParent: true,
            flatKeys: [],
        };
    }

    private initFlatItems(items: Array<NavItem>) {
        this.setState({
            flatKeys: this.props.items.selectMore(x => x.children).select(x => x.key),
        });
    };

    private getParentKey(key: string, tree = this.props.items): string {
        let parentKey: string;
        for (let i = 0; i < tree.length; i++) {
            const node = tree[i] as NavItem;
            if (node.children) {
                if (node.children.some(item => item.key === key)) {
                    parentKey = node.key;
                } else if (this.getParentKey(key, node.children)) {
                    parentKey = this.getParentKey(key, node.children);
                }
            }
        }
        return parentKey;
    };

    toggleKey(key: string) {
        this.setState({
            autoExpandParent: false,
        })

        let expandedKeys = this.state.expandedKeys;

        var index = this.state.expandedKeys.indexOf(key);
        if (index > -1) expandedKeys.splice(index, 1);
        else expandedKeys.push(key);

        this.setState({ expandedKeys });
    }

    onExpand(expandedKeys: string[]) {
        this.setState({
            expandedKeys,
            autoExpandParent: false,
        })
    }

    onChange(e: React.ChangeEvent<HTMLInputElement>) {
        const value = e.target.value;
        let expandedKeys: string[];

        if (value !== '') {
            expandedKeys = this.state.flatKeys
                .map(key => {
                    if (key.indexOf(value) > -1)
                        return this.getParentKey(key);
                    else return null;
                })
                .filter((item, i, self) => item && self.indexOf(item) === i);
        }
        else expandedKeys = [];

        Object.assign(this, {
            expandedKeys,
            searchValue: value,
            autoExpandParent: true,
        });
    }

    componentDidMount() {
        this.props.init?.(this);
        this.initFlatItems(this.props.items);
    }

    render() {
        var title_area = (item: NavItem) => {
            var key = item.key;
            var title = item.title;
            var content = title.indexOf(this.state.searchValue) > -1 ? (
                <span>
                    {title.substr(0, title.indexOf(this.state.searchValue))}
                    <span style={{ color: '#f50' }}>{this.state.searchValue}</span>
                    {title.substr(title.indexOf(this.state.searchValue) + this.state.searchValue.length)}
                </span>
            ) : (
                <span>{title}</span>
            );

            if (item.render != null) {
                return <span onClick={() => this.props.action(key)}>{content}</span>
            } else {
                return <span onClick={() => this.toggleKey(key)}>{content}</span>
            }
        };

        return (
            <div>
                <Input.Search style={{ marginBottom: 8, height: 40 }} placeholder="Search" onChange={e => this.onChange(e)} />
                <Tree
                    showIcon={true}
                    onExpand={expandedKeys => this.onExpand(expandedKeys as string[])}
                    expandedKeys={this.state.expandedKeys}
                    autoExpandParent={this.state.autoExpandParent}
                    treeData={this.props.items}
                    titleRender={(item: NavItem) => title_area(item)} />
            </div>
        );
    }
}
