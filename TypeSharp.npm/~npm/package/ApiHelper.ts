import axios, { AxiosResponse } from "axios";

export class ApiHelper {

    static default: ApiHelper = new ApiHelper();

    constructor(public handler?: {
        beforeResolve?: (response: AxiosResponse) => boolean | void,
        beforeReject?: (reason: any) => boolean | void
    }) {
    }

    static get<T>(url: string, params?: any): Promise<T> { return this.default.get(url, params); }
    static delete<T>(url: string, params?: any): Promise<T> { return this.default.delete(url, params); }
    static head<T>(url: string, params?: any): Promise<T> { return this.default.head(url, params); }
    static options<T>(url: string, params?: any): Promise<T> { return this.default.options(url, params); }
    static get_save(url: string, params?: any) { this.default.get_save(url, params); }

    static post<T>(url: string, data?: any, params?: any): Promise<T> { return this.default.post(url, data, params); }
    static put<T>(url: string, data?: any, params?: any): Promise<T> { return this.default.put(url, data, params); }
    static patch<T>(url: string, data?: any, params?: any): Promise<T> { return this.default.patch(url, data, params); }
    static post_save(url: string, data?: any, params?: any) { this.default.post_save(url, data, params); }

    get<T>(url: string, params?: any): Promise<T> { return this.handle(axios.get(url, { params: params })); }
    delete<T>(url: string, params?: any): Promise<T> { return this.handle(axios.delete(url, { params: params })); }
    head<T>(url: string, params?: any): Promise<T> { return this.handle(axios.head(url, { params: params })); }
    options<T>(url: string, params?: any): Promise<T> { return this.handle(axios.options(url, { params: params })); }
    get_save(url: string, params?: any): Promise<void> { return this.handle_save(axios.get(url, { params: params, responseType: 'blob' })); }

    post<T>(url: string, data?: any, params?: any): Promise<T> { return this.handle(axios.post(url, data, { params: params })); }
    put<T>(url: string, data?: any, params?: any): Promise<T> { return this.handle(axios.put(url, data, { params: params })); }
    patch<T>(url: string, data?: any, params?: any): Promise<T> { return this.handle(axios.patch(url, data, { params: params })); }
    post_save(url: string, data?: any, params?: any): Promise<void> { return this.handle_save(axios.post(url, data, { params: params, responseType: 'blob' })); }

    private handle<T>(axiosResponse: Promise<AxiosResponse>): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            axiosResponse
                .then(response => {
                    var status = this.handler?.beforeResolve?.call(this, response);
                    if (status !== false) resolve(response.data as T);
                })
                .catch(reason => {
                    var status = this.handler?.beforeReject?.call(this, reason);
                    if (status !== false) reject(reason);
                });
        });
    }

    private handle_save(axiosResponse: Promise<AxiosResponse>): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            axiosResponse
                .then(response => {
                    ApiHelper.save(response);
                    var status = this.handler?.beforeResolve?.call(this, response);
                    if (status !== false) resolve(response.data);
                })
                .catch(reason => {
                    var status = this.handler?.beforeReject?.call(this, reason);
                    if (status !== false) reject(reason);
                });
        });
    }

    private static save(response: AxiosResponse<any>) {
        var filename = ApiHelper.getFileName(response.headers['content-disposition']);
        ApiHelper.popupSaveDialog(response.data, filename);
    }

    private static popupSaveDialog(data: any, filename: string) {

        var blob = new Blob([data]);

        if (window.navigator['msSaveOrOpenBlob']) {
            window.navigator['msSaveOrOpenBlob'](blob, filename);
        } else {
            var element = document.createElement('a');
            var href = window.URL.createObjectURL(blob);
            element.href = href;
            element.download = filename;
            document.body.appendChild(element);
            element.click();
            document.body.removeChild(element);
            window.URL.revokeObjectURL(href);
        }
    }

    private static getFileName(header: string, defaultName: string = 'file') {

        if (header == null) return defaultName;

        var getName = (regex: RegExp) => {
            var match: RegExpExecArray;
            if ((match = regex.exec(header)) !== null)
                return decodeURI(match[1]);
            else return null;
        }

        return getName(/(?:filename\*=UTF-8'')([^;$]+)/g)
            ?? getName(/(?:filename=)([^;$]+)/g)
            ?? defaultName;
    }

}
