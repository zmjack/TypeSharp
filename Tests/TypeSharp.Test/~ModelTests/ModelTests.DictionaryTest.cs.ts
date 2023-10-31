declare namespace TypeSharp.Test.Models {
    interface DictionaryClass {
        dict1?: { [key: string]: number };
        dict2?: { [key: string]: { [key: string]: number } };
        dict3?: { [key: string]: SuperClass };
        dict4?: { [key: string]: SpecialNsClass };
    }
    interface SuperClass {
        nested?: SuperClass.NestedClass;
        overrideInt?: number;
        newObject?: any;
    }
    interface SpecialNsClass {
    }
}
declare namespace TypeSharp.Test.Models.SuperClass {
    interface NestedClass {
        intValue?: number;
    }
}
