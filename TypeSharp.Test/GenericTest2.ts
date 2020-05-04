declare namespace TSNS3 {
    interface GenericClass<T> {
        value?: T;
        values?: T[];
        values2?: T[][];
        intValue?: GenericClass<number>;
    }
    export const enum GenericClass_names {
        value = 'Value',
        values = 'Values',
        values2 = 'Values2',
        intValue = 'IntValue',
    }
}
