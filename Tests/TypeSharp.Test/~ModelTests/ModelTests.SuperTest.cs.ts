declare namespace TypeSharp.Test.Models {
    interface SuperClass {
        nested?: SuperClass.NestedClass;
        overrideInt?: number;
        newObject?: any;
    }
    interface ChildClass {
        overrideInt?: number;
        newObject?: SuperClass.NestedClass;
        nested?: SuperClass.NestedClass;
    }
}
declare namespace TypeSharp.Test.Models.SuperClass {
    interface NestedClass {
        intValue?: number;
    }
}
