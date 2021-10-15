declare namespace Ajax {
    interface JSend {
        status?: string;
        data?: any;
        message?: string;
        code?: string;
    }
}

namespace Ajax {
    export namespace JSend {
        export const Status_Success: string = 'success';
        export const Status_Fail: string = 'fail';
        export const Status_Error: string = 'error';
    }
}
