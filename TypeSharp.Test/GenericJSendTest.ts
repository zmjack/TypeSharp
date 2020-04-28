declare namespace Ajax {
    interface JSend<TData> {
        status?: string;
        code?: string;
        message?: string;
        data?: TData;
    }
}
