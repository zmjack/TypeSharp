import { ApiHelper, version } from '../lib'
if (typeof (window) != 'undefined') {
    (window as any).ver_type_sharp = version;
    (window as any).tsapi = ApiHelper;
}

