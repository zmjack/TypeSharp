declare namespace LiveAccount {
    interface LiveUser {
        id?: string;
        name?: string;
        issuer?: string;
        sub?: string;
        userRoles?: LiveUserRole<LiveUser, LiveRole>[];
    }
    interface LiveRole {
        id?: string;
        name?: string;
        rootGroup?: boolean;
        superiorRole?: string;
        superRoleLink?: LiveRole;
        inferiorRoles?: LiveRole[];
        userRoles?: LiveUserRole<LiveUser, LiveRole>[];
        roleOperations?: LiveRoleOperation<LiveUser, LiveRole>[];
        roleGrants?: LiveRoleGrantRole<LiveUser, LiveRole>[];
        roleGrantGroups?: LiveRoleGrantGroup<LiveUser, LiveRole>[];
        roleGroups?: LiveRoleGroup<LiveUser, LiveRole>[];
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
