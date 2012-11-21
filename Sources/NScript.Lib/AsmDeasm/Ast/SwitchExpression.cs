using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ast
{
    class SwitchExpression : BaseBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public SwitchExpression(
            Expression switchOn,
            List<KeyValuePair<List<object>, IList<IAstNode>>> caseStatements)
        {
            this.SwitchOn = switchOn;
            this.CaseStatements = caseStatements;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Block; }
        }

        public List<KeyValuePair<List<object>, IList<IAstNode>>> CaseStatements
        { get; private set; }

        public Expression SwitchOn
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}switch(",
                prefix);

            this.SwitchOn.Write(textWriter);

            textWriter.Write(
                "){0}{{",
                prefix);

            string newPrefix = prefix + "    ";
            for (int iCase = 0; iCase < this.CaseStatements.Count; iCase++)
            {
                for (int jCaseVar = 0; jCaseVar < this.CaseStatements[iCase].Key.Count; jCaseVar++)
                {
                    if (this.CaseStatements[iCase].Key[jCaseVar] is int)
                    {
                        textWriter.Write("{0}case {1}:",
                            prefix,
                            this.CaseStatements[iCase].Key[jCaseVar]);
                    }
                    else if (this.CaseStatements[iCase].Key[jCaseVar] == null)
                    {
                        textWriter.Write("{0}default:", prefix);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                for (int iExpression = 0; iExpression < this.CaseStatements[iCase].Value.Count; iExpression++)
                {
                    this.CaseStatements[iCase].Value[iExpression].Write(
                        textWriter,
                        newPrefix,
                        indentation);
                }
            }

            textWriter.Write(
                "{0}}}",
                prefix);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
