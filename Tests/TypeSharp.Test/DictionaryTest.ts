declare namespace TypeSharp.Test {
    interface DictionaryClass {
        dict1?: { [key: string]: number };
        dict2?: { [key: string]: { [key: string]: number } };
    }
}
