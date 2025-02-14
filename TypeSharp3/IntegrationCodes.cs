using System.Text;

namespace TypeSharp;

public static class IntegrationCode
{
    public const string SaveFile = "SaveFile";
    public const string SaveFile_DeclareOnly = "SaveFile.d";
    public const string HandleResponse_DeclareOnly = "HandleResponse.d";

    internal static string GetCode(string[] names)
    {
        var builder = new StringBuilder();
        foreach (var name in names)
        {
            var code = name switch
            {
                SaveFile => Code_SaveFile(),
                SaveFile_DeclareOnly => Code_SaveFile_DeclareOnly(),
                HandleResponse_DeclareOnly => Code_HandleResponse_DeclareOnly(),
                _ => throw new NotSupportedException($"Invalid code name. (Name: {name})"),
            };
            builder.AppendLine(code);
        }
        return builder.ToString();
    }

    internal static string Code_SaveFile()
    {
        return
            """
            const $ts_hcd = (function () {
              const rules = [/(?:filename\*=UTF-8'')([^;$]+)/, /(?:filename=)([^;$]+)/];
              return function (header: string) {
                if (header === void 0 || header === null) return void 0;
                for (let rule of rules) {
                  const match = rule.exec(header);
                  if (match !== null) return decodeURI(match[1]);
                }
                return void 0;
              }
            }());
            const $ts_save = (function () {
              if (window?.navigator !== void 0) {
                var save = (window.navigator as any)['msSaveOrOpenBlob'];
                if (save !== void 0) {
                  return function (blob: Blob, filename: string) {
                    save(blob, filename);
                  }
                }
              }
              return function (blob: Blob, filename: string) {
                var el = document.createElement('a');
                var href = window.URL.createObjectURL(blob);
                el.href = href;
                el.download = filename;
                document.body.appendChild(el);
                el.click();
                document.body.removeChild(el);
                window.URL.revokeObjectURL(href);
              };
            }());
            """;
    }

    internal static string Code_SaveFile_DeclareOnly()
    {
        return
            """
            declare var $ts_hcd: (header: string) => string;
            declare var $ts_save: (blob: Blob, filename: string) => void;
            """;
    }

    internal static string Code_HandleResponse_DeclareOnly()
    {
        return
            """
            declare var $ts_handle_response: (response: Response) => any;
            declare var $ts_handle_error: (reason: any) => void;
            """;
    }
}
