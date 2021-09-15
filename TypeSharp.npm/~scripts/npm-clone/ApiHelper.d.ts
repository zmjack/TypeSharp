import { AxiosResponse } from "axios";
export declare class ApiHelper {
    handler?: {
        beforeResolve?: (response: AxiosResponse) => boolean | void;
        beforeReject?: (reason: any) => boolean | void;
    };
    static default: ApiHelper;
    constructor(handler?: {
        beforeResolve?: (response: AxiosResponse) => boolean | void;
        beforeReject?: (reason: any) => boolean | void;
    });
    static get<T>(url: string, params?: any): Promise<T>;
    static delete<T>(url: string, params?: any): Promise<T>;
    static head<T>(url: string, params?: any): Promise<T>;
    static options<T>(url: string, params?: any): Promise<T>;
    static get_save(url: string, params?: any): void;
    static post<T>(url: string, data?: any, params?: any): Promise<T>;
    static put<T>(url: string, data?: any, params?: any): Promise<T>;
    static patch<T>(url: string, data?: any, params?: any): Promise<T>;
    static post_save(url: string, data?: any, params?: any): void;
    get<T>(url: string, params?: any): Promise<T>;
    delete<T>(url: string, params?: any): Promise<T>;
    head<T>(url: string, params?: any): Promise<T>;
    options<T>(url: string, params?: any): Promise<T>;
    get_save(url: string, params?: any): Promise<void>;
    post<T>(url: string, data?: any, params?: any): Promise<T>;
    put<T>(url: string, data?: any, params?: any): Promise<T>;
    patch<T>(url: string, data?: any, params?: any): Promise<T>;
    post_save(url: string, data?: any, params?: any): Promise<void>;
    private handle;
    private handle_save;
    private static save;
    private static popupSaveDialog;
    private static getFileName;
}
