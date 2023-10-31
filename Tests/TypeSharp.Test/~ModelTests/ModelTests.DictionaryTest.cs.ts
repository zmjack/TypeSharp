declare namespace TypeSharp.Test.Models {
    interface DictionaryClass {
        dict1?: { [key: string | number]: number };
        dict2?: { [key: string | number]: { [key: string | number]: number } };
        dict3?: { [key: string | number]: SuperClass };
        dict4?: { [key: string | number]: SpecialNsClass };
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
