﻿namespace TypeSharp.DevAnalyzer;

internal static class Config
{
    internal const string RootNamespace = "TypeSharp.AST";

    internal static void Debugger()
    {
#if DEBUGANALYZER
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif
    }
}
