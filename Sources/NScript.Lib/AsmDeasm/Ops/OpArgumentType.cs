using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ops
{
    public enum OpArgumentType
    {
        None,
        ArgumentId,
        LocalVariableId,
        Field,
        Method,
        BranchTarget,
        SwitchTargets,
        Constant,
        ConstantDouble,
        ConstantString,
        ObjectType,
    }
}
