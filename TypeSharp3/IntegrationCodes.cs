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
            function $ts_hcd(header: string): string | undefined {
              if (header === null || header === void 0) return undefined;
              var name = (regex: RegExp) => {
                var match: RegExpExecArray | null;
                if ((match = regex.exec(header)) !== null)
                  return decodeURI(match[1]);
                else return undefined;
              }
              return name(/(?:filename\*=UTF-8'')([^;$]+)/g) ?? name(/(?:filename=)([^;$]+)/g);
            }
            function $ts_save(blob: Blob, filename: string): void {
              if (typeof window.navigator !== 'undefined') {
                var save = (window.navigator as any)['msSaveOrOpenBlob'];
                save(blob, filename);
              } else {
                var el = document.createElement('a');
                var href = window.URL.createObjectURL(blob);
                el.href = href;
                el.download = filename;
                document.body.appendChild(el);
                el.click();
                document.body.removeChild(el);
                window.URL.revokeObjectURL(href);
              }
            }
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
