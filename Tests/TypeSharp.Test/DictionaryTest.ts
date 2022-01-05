declare namespace TypeSharp.Test {
    interface DictionaryClass {
        dict1?: { [key: string]: number };
        dict2?: { [key: string]: { [key: string]: number } };
        dict3?: { [key: string]: SuperClass };
        dict4?: { [key: string]: Special.SpecialNsClass };
    }
    interface SuperClass {
        nested?: SuperClass.NestedClass;
        overrideInt?: number;
        newObject?: any;
    }
}
declare namespace TypeSharp.Test.Special {
    interface SpecialNsClass {
    }
}
declare namespace TypeSharp.Test.SuperClass {
    interface NestedClass {
        intValue?: number;
    }
}
