using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs2JsC.Lib.AsmDeasm.Ops
{
    public class OpCodeWrapper
    {
        #region member variables
        readonly private Regex matchRegEx;
        readonly private FlowType flow;

        readonly private IlOpCode opCode;
        readonly private OpArgumentType argumentType;
        readonly private StackPushCode pushCode;
        readonly private StackPopCode popCode;

        readonly private static Dictionary<string, List<OpCodeWrapper>> opcodeBuckets =
            new Dictionary<string, List<OpCodeWrapper>>();
        readonly private static Dictionary<IlOpCode, OpCodeWrapper> ilOpCodes =
            new Dictionary<IlOpCode, OpCodeWrapper>();
        #endregion

        #region constructors/Factories
        internal OpCodeWrapper(
            string matchRegex,
            IlOpCode ilOpCode,
            FlowType flow,
            OpArgumentType argumentType,
            StackPushCode pushCode,
            StackPopCode popCode)
        {
            this.opCode = ilOpCode;
            this.matchRegEx = new Regex(
                string.Format("^{0}$", matchRegex),
                RegexOptions.Compiled);
            this.flow = flow;
            this.argumentType = argumentType;
            this.pushCode = pushCode;
            this.popCode = popCode;

            OpCodeWrapper.RegisterOpCode(this, matchRegex);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Regex MatchRegEx
        { get { return this.matchRegEx; } }

        public FlowType Flow
        { get { return this.flow; } }

        public IlOpCode Code
        { get { return this.opCode; } }

        public OpArgumentType ArgumentType
        { get { return this.argumentType; } }

        public StackPushCode PushCode
        { get { return this.pushCode; } }

        public StackPopCode PopCode
        { get { return this.popCode; } }
        #endregion

        #region public functions
        public static OpCodeWrapper GetOpCode(string opCodeStr)
        {
            var returnValue = OpCodes.Add;
            List<OpCodeWrapper> list = null;
            string opCodePrefix = opCodeStr.Split('.')[0];

            if (OpCodeWrapper.opcodeBuckets.TryGetValue(opCodePrefix, out list))
            {
                for (int iList = 0; iList < list.Count; iList++)
                {
                    if (list[iList].MatchRegEx.IsMatch(opCodeStr))
                    {
                        return list[iList];
                    }
                }
            }

            return null;
        }

        public static OpCodeWrapper GetOpCode(IlOpCode opCode)
        {
            return OpCodeWrapper.ilOpCodes[opCode];
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}, {3}",
                this.Code,
                this.Flow,
                this.PushCode,
                this.PopCode);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static void RegisterOpCode(
            OpCodeWrapper opCode,
            string matchStr)
        {
            string prefix = matchStr.Split('\\', '(')[0];
 
            List<OpCodeWrapper> list = null;
            if (!OpCodeWrapper.opcodeBuckets.TryGetValue(prefix, out list))
            {
                list = new List<OpCodeWrapper>();
                OpCodeWrapper.opcodeBuckets.Add(prefix, list);
            }

            list.Add(opCode);

            OpCodeWrapper.ilOpCodes.Add(opCode.Code, opCode);
        }
        #endregion
    }
}
