using JsCsc.Lib.Serialization;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScript.Csc.Lib
{
    public static class RoslynExtensions
    {
        public static LocationSer GetSerLoc(this SyntaxNode syntaxNode)
        {
            if (syntaxNode?.Location == null)
            { return null; }

            return GetSerLoc(syntaxNode.Location);
        }

        public static LocationSer GetSerLoc(this Location location)
        {
            var lineMappig = location.GetMappedLineSpan();
            return new LocationSer
            {
                StartColumn = lineMappig.StartLinePosition.Character + 1,
                StartLine = lineMappig.StartLinePosition.Line + 1,
                EndColumn = lineMappig.EndLinePosition.Character + 1,
                EndLine = lineMappig.EndLinePosition.Line + 1,
            };
        }
    }
}
