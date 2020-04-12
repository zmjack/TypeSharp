import { ApiHelper } from "type-sharp"

export class SimpleController {
    static getUsers(group: string): Promise<TSNS2.SubClass[]> { return ApiHelper.get('/com/Simple/GetUsers', { group }); }
    static getAction(groupId: number): Promise<number> { return ApiHelper.get('/com/Simple/GetAction', { groupId }); }
    static postAction(model: TSNS1.RootClass): Promise<number> { return ApiHelper.post('/com/Simple/PostAction', model, { }); }
}
