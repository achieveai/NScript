using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.Ops
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
