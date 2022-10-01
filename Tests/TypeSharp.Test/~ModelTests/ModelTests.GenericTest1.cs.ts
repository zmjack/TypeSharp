declare namespace TSNS3 {
    interface GenericClass<T> {
        value?: T;
        values?: T[];
        values2?: T[][];
        intValue?: GenericClass<number>;
    }
}
