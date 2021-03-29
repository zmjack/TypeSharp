declare namespace TSNS1 {
    interface RootClass {
        state?: TypeSharp.Test.EState;
        subClass?: TSNS2.SubClass;
        subClasses?: TSNS2.SubClass[];
        subStruct?: TSNS2.SubStruct;
        nullableSubStruct?: TSNS2.SubStruct;
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
    interface SubStruct {
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
