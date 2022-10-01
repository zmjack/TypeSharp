import { ApiHelper } from "type-sharp";

export class SimpleApi {
    constructor(public api: ApiHelper = ApiHelper.default) { }
    getUsers(group: string): Promise<TSNS2.SubClass[]> { return this.api.get('/com/Simple/GetUsers', { group }); }
    getAction(groupId: number): Promise<number> { return this.api.get('/com/Simple/GetAction', { groupId }); }
    postAction(model: TSNS1.RootClass): Promise<number> { return this.api.post('/com/Simple/PostAction', model, {}); }
    getFile(groupId: number): Promise<void> { return this.api.get_save('/com/Simple/GetFile', { groupId }); }
    postFile(model: TSNS1.RootClass): Promise<void> { return this.api.post_save('/com/Simple/PostFile', model, {}); }
}
