declare namespace TypeSharp.Test {
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
declare namespace TypeSharp.Test.SuperClass {
    interface NestedClass {
        intValue?: number;
    }
}
