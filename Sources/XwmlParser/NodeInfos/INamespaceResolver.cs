//-----------------------------------------------------------------------
// <copyright file="INamespaceResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    interface INamespaceResolver
    {
        string GetFullNamespace(string abbr);

        string GetFullName(string abbrWithName);
    }
}
