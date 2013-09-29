//-----------------------------------------------------------------------
// <copyright file="Helper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using NScript.CLR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for Helper
    /// </summary>
    public class Helper
    {
        const string MsCorLib = "MsCorLib.dll";
        const string SystemWeb = "System.Web.dll";
        const string SystemWebHtml = "System.Web.Html.dll";
        const string SunlightFramework = "Sunlight.Framework.dll";
        const string SunlightFrameworkUi = "Sunlight.Framework.UI.dll";
        const string SunlightUnit = "SunlightUnit.dll";
        const string SunlightFrameworkTest = "Sunlight.Framework.Test.dll";
        const string SunlightFrameworkUITest = "Sunlight.Framework.UI.Test.dll";

        static ClrContext context;
        static TypeResolver resolver;

        public static void Initialize()
        {
            if (context == null)
            {
                context = new ClrContext();
                context.LoadAssembly(MsCorLib, false);
                context.LoadAssembly(SystemWeb, false);
                context.LoadAssembly(SystemWebHtml, false);
                context.LoadAssembly(SunlightFramework, false);
                context.LoadAssembly(SunlightUnit, false);
                context.LoadAssembly(SunlightFrameworkUi, false);
                context.LoadAssembly(SunlightFrameworkUITest, false);

                resolver = new TypeResolver(context);
            }
        }

        public static TypeResolver Resolver
        { get { return Helper.resolver; } }
    }
}
