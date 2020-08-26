declare namespace TSNS1 {
    interface RootClass {
        state?: TypeSharp.Test.EState;
        sub?: TSNS2.SubClass;
        subs?: TSNS2.SubClass[];
        str: string;
        int: number;
        strArray?: string[];
        nGuid?: string;
        dateTime?: Date;
        dateTimeOffset?: Date;
    }
}
declare namespace Ajax {
    interface JSend {
        status?: string;
        data?: any;
        code?: string;
        message?: string;
    }
}
declare namespace TypeSharp.Test {
    export const enum EState {
        Ready = 0,
        Running = 1,
        Complete = 2,
    }
}
declare namespace TSNS2 {
    interface SubClass {
        name?: string;
        value?: string;
        members?: string[];
    }
}

namespace TSNS1 {
    export namespace RootClass {
        export const CONST_STRING: string = 'const_string';
        export const CONST_INTEGER: number = 2147483647;
    }
}
