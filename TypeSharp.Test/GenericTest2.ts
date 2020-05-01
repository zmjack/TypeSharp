declare namespace TSNS3 {
    interface GenericClass<T> {
        value?: T;
        intValue?: GenericClass<number>;
    }
    export const enum GenericClass_names {
        value = 'Value',
        intValue = 'IntValue',
    }
}
