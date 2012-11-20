using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ops
{
    public enum FlowType
    {
        Switch,
        Branch,
        ConditionalBranch,
        MethodCall,
        Next,
        Throw,
        Return,
        Unsuported,
    }
}
