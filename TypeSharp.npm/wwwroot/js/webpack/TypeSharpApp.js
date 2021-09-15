(()=>{"use strict";var e={45:function(e,t,n){var o=this&&this.__createBinding||(Object.create?function(e,t,n,o){void 0===o&&(o=n),Object.defineProperty(e,o,{enumerable:!0,get:function(){return t[n]}})}:function(e,t,n,o){void 0===o&&(o=n),e[o]=t[n]}),i=this&&this.__setModuleDefault||(Object.create?function(e,t){Object.defineProperty(e,"default",{enumerable:!0,value:t})}:function(e,t){e.default=t}),r=this&&this.__importStar||function(e){if(e&&e.__esModule)return e;var t={};if(null!=e)for(var n in e)Object.hasOwnProperty.call(e,n)&&o(t,e,n);return i(t,e),t};Object.defineProperty(t,"__esModule",{value:!0});const a=r(n(804)),l=r(n(196)),u=n(333);l.render(a.createElement(u.TypeSharpPanel,null),document.getElementById("app"))},333:function(e,t,n){var o=this&&this.__createBinding||(Object.create?function(e,t,n,o){void 0===o&&(o=n),Object.defineProperty(e,o,{enumerable:!0,get:function(){return t[n]}})}:function(e,t,n,o){void 0===o&&(o=n),e[o]=t[n]}),i=this&&this.__setModuleDefault||(Object.create?function(e,t){Object.defineProperty(e,"default",{enumerable:!0,value:t})}:function(e,t){e.default=t}),r=this&&this.__importStar||function(e){if(e&&e.__esModule)return e;var t={};if(null!=e)for(var n in e)Object.hasOwnProperty.call(e,n)&&o(t,e,n);return i(t,e),t};Object.defineProperty(t,"__esModule",{value:!0}),t.TypeSharpPanel=void 0;const a=r(n(804)),l=n(398),u=n(109);class s extends a.Component{constructor(e){super(e),this.api=new u.HomeApi(new l.ApiHelper({beforeResolve:e=>{var t=e.data;"[object Blob]"==(null==t?void 0:t.toString())?this.setState({content:"[Download file]"}):this.setState({content:t})},beforeReject:e=>{this.setState({content:`${e}`})}})),this.state={content:"<null>"}}render(){return a.createElement("div",null,a.createElement("div",null,a.createElement("button",{type:"button",onClick:()=>this.api.getContent()},"GetContent"),a.createElement("button",{type:"button",onClick:()=>this.api.getContent500()},"GetContent500"),a.createElement("button",{type:"button",onClick:()=>this.api.getFile()},"GetFile"),a.createElement("button",{type:"button",onClick:()=>this.api.getFile404()},"GetFile404"),a.createElement("button",{type:"button",onClick:()=>this.api.getFile500()},"GetFile500")),a.createElement("div",null,"content: ",this.state.content))}}t.TypeSharpPanel=s},109:(e,t,n)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.HomeApi=void 0;const o=n(398);t.HomeApi=class{constructor(e=o.ApiHelper.default){this.api=e}index(){return this.api.get("/Home/Index",{})}privacy(){return this.api.get("/Home/Privacy",{})}getContent(){return this.api.get("/Home/GetContent",{})}getContent500(){return this.api.get("/Home/GetContent500",{})}getFile(){return this.api.get_save("/Home/GetFile",{})}getFile404(){return this.api.get_save("/Home/GetFile404",{})}getFile500(){return this.api.get_save("/Home/GetFile500",{})}error(){return this.api.get("/Home/Error",{})}}},664:(e,t,n)=>{t.__esModule=!0,t.ApiHelper=void 0;var o=n(376),i=function(){function e(e){this.handler=e}return e.get=function(e,t){return this.default.get(e,t)},e.delete=function(e,t){return this.default.delete(e,t)},e.head=function(e,t){return this.default.head(e,t)},e.options=function(e,t){return this.default.options(e,t)},e.get_save=function(e,t){this.default.get_save(e,t)},e.post=function(e,t,n){return this.default.post(e,t,n)},e.put=function(e,t,n){return this.default.put(e,t,n)},e.patch=function(e,t,n){return this.default.patch(e,t,n)},e.post_save=function(e,t,n){this.default.post_save(e,t,n)},e.prototype.get=function(e,t){return this.handle(o.default.get(e,{params:t}))},e.prototype.delete=function(e,t){return this.handle(o.default.delete(e,{params:t}))},e.prototype.head=function(e,t){return this.handle(o.default.head(e,{params:t}))},e.prototype.options=function(e,t){return this.handle(o.default.options(e,{params:t}))},e.prototype.get_save=function(e,t){return this.handle_save(o.default.get(e,{params:t,responseType:"blob"}))},e.prototype.post=function(e,t,n){return this.handle(o.default.post(e,t,{params:n}))},e.prototype.put=function(e,t,n){return this.handle(o.default.put(e,t,{params:n}))},e.prototype.patch=function(e,t,n){return this.handle(o.default.patch(e,t,{params:n}))},e.prototype.post_save=function(e,t,n){return this.handle_save(o.default.post(e,t,{params:n,responseType:"blob"}))},e.prototype.handle=function(e){var t=this;return new Promise((function(n,o){e.then((function(e){var o,i;!1!==(null===(i=null===(o=t.handler)||void 0===o?void 0:o.beforeResolve)||void 0===i?void 0:i.call(t,e))&&n(e.data)})).catch((function(e){var n,i;!1!==(null===(i=null===(n=t.handler)||void 0===n?void 0:n.beforeReject)||void 0===i?void 0:i.call(t,e))&&o(e)}))}))},e.prototype.handle_save=function(t){var n=this;return new Promise((function(o,i){t.then((function(t){var i,r;e.save(t),!1!==(null===(r=null===(i=n.handler)||void 0===i?void 0:i.beforeResolve)||void 0===r?void 0:r.call(n,t))&&o(t.data)})).catch((function(e){var t,o;!1!==(null===(o=null===(t=n.handler)||void 0===t?void 0:t.beforeReject)||void 0===o?void 0:o.call(n,e))&&i(e)}))}))},e.save=function(t){var n=e.getFileName(t.headers["content-disposition"]);e.popupSaveDialog(t.data,n)},e.popupSaveDialog=function(e,t){var n=new Blob([e]);if(window.navigator.msSaveOrOpenBlob)window.navigator.msSaveOrOpenBlob(n,t);else{var o=document.createElement("a"),i=window.URL.createObjectURL(n);o.href=i,o.download=t,document.body.appendChild(o),o.click(),document.body.removeChild(o),window.URL.revokeObjectURL(i)}},e.getFileName=function(e,t){var n,o;if(void 0===t&&(t="file"),null==e)return t;var i=function(t){var n;return null!==(n=t.exec(e))?decodeURI(n[1]):null};return null!==(o=null!==(n=i(/(?:filename\*=UTF-8'')([^;$]+)/g))&&void 0!==n?n:i(/(?:filename=)([^;$]+)/g))&&void 0!==o?o:t},e.default=new e,e}();t.ApiHelper=i},398:(e,t,n)=>{t.__esModule=!0,t.version=t.ApiHelper=void 0;var o=n(664);t.ApiHelper=o.ApiHelper,t.version="0.1.1"},804:e=>{e.exports=React},196:e=>{e.exports=ReactDOM},376:e=>{e.exports=axios}},t={};!function n(o){var i=t[o];if(void 0!==i)return i.exports;var r=t[o]={exports:{}};return e[o].call(r.exports,r,r.exports,n),r.exports}(45)})();
//# sourceMappingURL=TypeSharpApp.js.map