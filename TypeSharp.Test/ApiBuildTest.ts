import { ApiHelper } from "type-sharp"

export class SimpleController {
    static getUsers(group: string): Promise<TSNS2.SubClass[]> { return ApiHelper.get('Simple/GetUsers', { group }); }
    static getAction(groupId: number): Promise<number> { return ApiHelper.get('Simple/GetAction', { groupId }); }
    static postAction(model: TSNS1.RootClass): Promise<number> { return ApiHelper.post('Simple/PostAction', model, { }); }
}
