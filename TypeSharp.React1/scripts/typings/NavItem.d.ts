
interface NavItem {
    title: string;
    key: string;
    render?: (h) => void;
    children?: Array<NavItem>;
    scopedSlots?: { title?: string, icon?: string };
}
