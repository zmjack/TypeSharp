declare namespace TypeSharp.Test.Models {
    interface DictionaryClass {
        dict1?: { [key: string | number | symbol]: number };
        dict2?: { [key: string | number | symbol]: { [key: string | number | symbol]: number } };
        dict3?: { [key: string | number | symbol]: SuperClass };
        dict4?: { [key: string | number | symbol]: SpecialNsClass };
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
