using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ops
{
    public enum StackPopCode
    {
        Pop0,
        Pop1,
        Pop2,
        Pop3,
        PopVar,
        PopAll
    }

    public enum StackPushCode
    {
        Push0,
        Push1,
        Push2,
        PushVar
    }
}
