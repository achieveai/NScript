using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;
using System.IO;

namespace NScript.Lib.AsmDeasm.Ast
{
    internal class ObjectConstructor : Expression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ObjectConstructor(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base (context, expressions)
        {
            this.Method = (MethodSignature)instruction.Instruction.OpCodeArgument;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Constructor; }
        }

        public MethodSignature Method
        { get; private set; }

        #endregion

        #region public functions
        public override void Write(
            TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.Context.IsDelegateConstructor(this.Method))
            {
                this.WriteDelegateConstructor(textWriter, prefix, indentation);
            }
            else if (this.Context.IsDefaultConstructor(this.Method))
            {
                this.WriteDefaultConcstructor(textWriter, prefix, indentation);
            }
            else
            {
                this.WriteConstructor(textWriter, prefix, indentation);
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private void WriteDelegateConstructor(
            TextWriter textWriter,
            string prefix,
            string indentation)
        {
            textWriter.Write("{0}Delegate.create(", prefix);
            this.WriteArguments(textWriter);
            textWriter.Write(")");
        }

        private void WriteDefaultConcstructor(
            TextWriter writer,
            string prefix,
            string indentation)
        {
            throw new NotImplementedException();
        }

        private void WriteConstructor(
            TextWriter textWriter,
            string prefix,
            string indentation)
        {
            textWriter.Write("{0}(new {1}()).{2}(",
                prefix,
                this.Context.ResolveClassName(this.Method.Class),
                this.Context.ResolveMethodName(this.Method));
            this.WriteArguments(textWriter);
            textWriter.Write(")");
        }

        private void WriteArguments(
            TextWriter textWriter)
        {
            for (int i = 0; i < this.Method.Arguments.Count; i++)
            {
                if (i != 0)
                {
                    textWriter.Write(", ");
                }

                this.DependentExpressions[i].Write(textWriter);
            }
        }
        #endregion
    }
}
