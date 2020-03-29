declare namespace LiveAccount {
    interface LiveUser {
        id?: string;
        sub?: string;
        name?: string;
        userRoles?: LiveUserRole<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
    }
    interface LiveRole {
        id?: string;
        name?: string;
        rootGroup?: boolean;
        superiorRole?: string;
        superRoleLink?: LiveRole;
        inferiorRoles?: LiveRole[];
        userRoles?: LiveUserRole<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
        roleOperations?: LiveRoleOperation<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
        roleGrants?: LiveRoleGrantRole<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
        roleGrantGroups?: LiveRoleGrantGroup<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
        roleGroups?: LiveRoleGroup<LiveAccount.LiveUser, LiveAccount.LiveRole>[];
    }
    interface LiveUserRole<TLiveUser, TLiveRole> {
        id?: string;
        user?: string;
        role?: string;
        userLink?: TLiveUser;
        roleLink?: TLiveRole;
    }
    interface LiveRoleOperation<TLiveUser, TLiveRole> {
        id?: string;
        role?: string;
        operation?: string;
        operationLink?: LiveOperation<TLiveUser, TLiveRole>;
        roleLink?: TLiveRole;
    }
    interface LiveRoleGrantRole<TLiveUser, TLiveRole> {
        id?: string;
        role?: string;
        grantRole?: string;
        roleLink?: TLiveRole;
        grantRoleLink?: LiveRoleGrantRole<TLiveUser, TLiveRole>;
    }
    interface LiveRoleGrantGroup<TLiveUser, TLiveRole> {
        id?: string;
        role?: string;
        grantGroup?: string;
        roleLink?: TLiveRole;
        grantGroupLink?: LiveGroup;
    }
    interface LiveRoleGroup<TLiveUser, TLiveRole> {
        id?: string;
        group?: string;
        role?: string;
        value?: string;
        groupLink?: LiveGroup;
        roleLink?: TLiveRole;
    }
    interface LiveOperation<TLiveUser, TLiveRole> {
        id?: string;
        name?: string;
        operationActions?: LiveOperationAction<TLiveUser, TLiveRole>[];
        roleOperations?: LiveRoleOperation<TLiveUser, TLiveRole>[];
    }
    interface LiveGroup {
        name?: string;
    }
    interface LiveOperationAction<TLiveUser, TLiveRole> {
        id?: string;
        operation?: string;
        action?: string;
        operationLink?: LiveOperation<TLiveUser, TLiveRole>;
        actionLink?: LiveAction<TLiveUser, TLiveRole>;
    }
    interface LiveAction<TLiveUser, TLiveRole> {
        id?: string;
        area?: string;
        controller?: string;
        action?: string;
        isExisted?: boolean;
        isEnabled?: boolean;
        name?: string;
        operationActions?: LiveOperationAction<TLiveUser, TLiveRole>[];
    }
}
