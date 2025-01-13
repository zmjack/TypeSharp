"use strict";
exports.__esModule = true;
exports.ApiHelper = void 0;
var axios_1 = require("axios");
var ApiHelper = /** @class */ (function () {
    function ApiHelper(handler) {
        this.handler = handler;
    }
    ApiHelper.get = function (url, params) { return this["default"].get(url, params); };
    ApiHelper["delete"] = function (url, params) { return this["default"]["delete"](url, params); };
    ApiHelper.head = function (url, params) { return this["default"].head(url, params); };
    ApiHelper.options = function (url, params) { return this["default"].options(url, params); };
    ApiHelper.get_save = function (url, params) { this["default"].get_save(url, params); };
    ApiHelper.post = function (url, data, params) { return this["default"].post(url, data, params); };
    ApiHelper.put = function (url, data, params) { return this["default"].put(url, data, params); };
    ApiHelper.patch = function (url, data, params) { return this["default"].patch(url, data, params); };
    ApiHelper.post_save = function (url, data, params) { this["default"].post_save(url, data, params); };
    ApiHelper.prototype.get = function (url, params) { return this.handle(axios_1["default"].get(url, { params: params })); };
    ApiHelper.prototype["delete"] = function (url, params) { return this.handle(axios_1["default"]["delete"](url, { params: params })); };
    ApiHelper.prototype.head = function (url, params) { return this.handle(axios_1["default"].head(url, { params: params })); };
    ApiHelper.prototype.options = function (url, params) { return this.handle(axios_1["default"].options(url, { params: params })); };
    ApiHelper.prototype.get_save = function (url, params) { return this.handle_save(axios_1["default"].get(url, { params: params, responseType: 'blob' })); };
    ApiHelper.prototype.post = function (url, data, params) { return this.handle(axios_1["default"].post(url, data, { params: params })); };
    ApiHelper.prototype.put = function (url, data, params) { return this.handle(axios_1["default"].put(url, data, { params: params })); };
    ApiHelper.prototype.patch = function (url, data, params) { return this.handle(axios_1["default"].patch(url, data, { params: params })); };
    ApiHelper.prototype.post_save = function (url, data, params) { return this.handle_save(axios_1["default"].post(url, data, { params: params, responseType: 'blob' })); };
    ApiHelper.prototype.handle = function (axiosResponse) {
        var _this = this;
        return new Promise(function (resolve, reject) {
            axiosResponse
                .then(function (response) {
                var _a, _b;
                var status = (_b = (_a = _this.handler) === null || _a === void 0 ? void 0 : _a.beforeResolve) === null || _b === void 0 ? void 0 : _b.call(_this, response);
                if (status !== false)
                    resolve(response.data);
            })["catch"](function (reason) {
                var _a, _b;
                var status = (_b = (_a = _this.handler) === null || _a === void 0 ? void 0 : _a.beforeReject) === null || _b === void 0 ? void 0 : _b.call(_this, reason);
                if (status !== false)
                    reject(reason);
            });
        });
    };
    ApiHelper.prototype.handle_save = function (axiosResponse) {
        var _this = this;
        return new Promise(function (resolve, reject) {
            axiosResponse
                .then(function (response) {
                var _a, _b;
                var status = (_b = (_a = _this.handler) === null || _a === void 0 ? void 0 : _a.beforeResolve) === null || _b === void 0 ? void 0 : _b.call(_this, response);
                if (status !== false) {
                    ApiHelper.save(response);
                    resolve(response.data);
                }
            })["catch"](function (reason) {
                var _a, _b;
                var status = (_b = (_a = _this.handler) === null || _a === void 0 ? void 0 : _a.beforeReject) === null || _b === void 0 ? void 0 : _b.call(_this, reason);
                if (status !== false)
                    reject(reason);
            });
        });
    };
    ApiHelper.save = function (response) {
        var filename = ApiHelper.getFileName(response.headers['content-disposition']);
        ApiHelper.popupSaveDialog(response.data, filename);
    };
    ApiHelper.popupSaveDialog = function (data, filename) {
        var blob = new Blob([data]);
        if (window.navigator['msSaveOrOpenBlob']) {
            window.navigator['msSaveOrOpenBlob'](blob, filename);
        }
        else {
            var element = document.createElement('a');
            var href = window.URL.createObjectURL(blob);
            element.href = href;
            element.download = filename;
            document.body.appendChild(element);
            element.click();
            document.body.removeChild(element);
            window.URL.revokeObjectURL(href);
        }
    };
    ApiHelper.getFileName = function (header, defaultName) {
        var _a, _b;
        if (defaultName === void 0) { defaultName = 'file'; }
        if (header == null)
            return defaultName;
        var getName = function (regex) {
            var match;
            if ((match = regex.exec(header)) !== null)
                return decodeURI(match[1]);
            else
                return null;
        };
        return (_b = (_a = getName(/(?:filename\*=UTF-8'')([^;$]+)/g)) !== null && _a !== void 0 ? _a : getName(/(?:filename=)([^;$]+)/g)) !== null && _b !== void 0 ? _b : defaultName;
    };
    ApiHelper["default"] = new ApiHelper();
    return ApiHelper;
}());
exports.ApiHelper = ApiHelper;
