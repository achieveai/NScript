using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;

namespace NScript.Lib.ILParser
{
    class ExecutionContext
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ExecutionContext(
            MethodSignature methodSignature,
            IList<ArgumentSignature> arguments,
            IList<ArgumentSignature> variables)
        {
            this.AssemblyContext = new AssemblyContext();
            this.MethodSignature = methodSignature;
            this.Arguments = arguments;
            this.Variables = variables;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public IList<ArgumentSignature> Arguments
        { get; private set; }

        public IList<ArgumentSignature> Variables
        { get; private set; }

        public MethodSignature MethodSignature
        { get; private set; }

        public AssemblyContext AssemblyContext
        { get; private set; }
        #endregion

        #region public functions
        internal string ResolveMethodName(MethodSignature methodSignature)
        {
            return NameResolver.Instance.GetMemberName(methodSignature);
        }

        internal string ResolveVirutalMethodName(MethodSignature methodSignature)
        {
            return NameResolver.Instance.GetVirtualMethodName(methodSignature);
        }

        internal string ResolveClassName(ClassSignature classSignature)
        {
            return NameResolver.Instance.GetClassName(classSignature);
        }

        internal bool IsDelegateConstructor(MethodSignature methodSignature)
        {
            return NameResolver.Instance.ResolveMethod(methodSignature).IsDelegateConstructor;
        }

        internal bool IsDefaultConstructor(MethodSignature methodSignature)
        {
            return false;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion


        internal object ResolveFieldName(FieldSignature fieldSignature)
        {
            return NameResolver.Instance.GetMemberName(fieldSignature);
        }
    }
}
