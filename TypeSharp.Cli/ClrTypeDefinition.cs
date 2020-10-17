using NStandard;
using NStandard.Caching;
using System;
using System.Linq;
using System.Reflection;

namespace TypeSharp
{
    public class ClrTypeDefinition
    {
        public static readonly string[] CliReferencedAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(x => x.Name).ToArray();
        public static readonly Assembly CoreLibAssembly = AppDomain.CurrentDomain.GetCoreLibAssembly();
        public string TargetBinFolder { get; set; }

        protected Cache<Assembly> AssemblyCache;
        protected Cache<Assembly> CachedType;

        public ClrTypeDefinition()
        {
            BindAssemblyCache();
        }

        public ClrTypeDefinition(string targetBinFolder, string typeString, string defaultAssemblyName)
        {
            TargetBinFolder = targetBinFolder;
            if (typeString.Count(",") == 1)
            {
                var parts = typeString.Split(',');
                TypeString = typeString;
                TypeName = parts[0];
                AssemblyName = parts[1];
            }
            else
            {
                TypeString = typeString;
                TypeName = typeString;
                AssemblyName = defaultAssemblyName;
            }
            IsValid = true;
            BindAssemblyCache();
        }

        private void BindAssemblyCache()
        {
            AssemblyCache = new Cache<Assembly>
            {
                CacheMethod = () =>
                {
                    var dll = $"{TargetBinFolder}/{AssemblyName}.dll";
                    var assembly = Assembly.LoadFile(dll);
                    return assembly;
                },
            };
        }

        public bool IsValid { get; set; }
        public string TypeString { get; set; }
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }

        public Type Type
        {
            get
            {
                if (CliReferencedAssemblyNames.Contains(AssemblyName))
                {
                    var type = Type.GetType(TypeString);
                    if (type == null) Console.Error.WriteLine($"Can not resolve(#1): {TypeString}");
                    return type;
                }
                else
                {
                    if (IsValid)
                    {
                        var assembly = AssemblyCache.Value;
                        var type = assembly.GetType(TypeName) ?? CoreLibAssembly.GetType(TypeName);
                        if (type == null) Console.Error.WriteLine($"Can not resolve(#2): {TypeString}");
                        return type;
                    }
                    else return null;
                }
            }
        }

    }
}
