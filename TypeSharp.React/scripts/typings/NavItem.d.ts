
interface NavItem {
    title: string;
    key: string;
    render?: JSX.Element;
    children?: Array<NavItem>;
}
