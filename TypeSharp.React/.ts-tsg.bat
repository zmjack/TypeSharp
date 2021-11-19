call dotnet ts tsg -o scripts/typings/@auto -n ^
-i "IdentityServer4.EntityFramework.Entities.ApiResource,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.Client,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.IdentityResource,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ApiResourceClaim,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ApiResourceProperty,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ApiScope,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ApiSecret,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientClaim,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientCorsOrigin,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientGrantType,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientIdPRestriction,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientProperty,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientRedirectUri,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientScope,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ClientSecret,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.IdentityClaim,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.IdentityResourceProperty,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.ApiScopeClaim,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.PersistedGrant,IdentityServer4.EntityFramework.Storage" ^
-i "IdentityServer4.EntityFramework.Entities.DeviceFlowCodes,IdentityServer4.EntityFramework.Storage" ^
-i "NStandard.LabelValuePair`1,NStandard" ^
-i "TypeSharp.Antd.TableFetchParams,TypeSharp.Antd" ^
-i "Ajax.JSend`1,JSend" ^
-r "Ajax.JSend,JSend;Ajax.JSend<any>" ^

call dotnet ts tsapi -o scripts/components ^
-r "Ajax.JSend,JSend;Ajax.JSend<any>" ^
