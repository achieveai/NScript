using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Template.Compiler.PropertyBuilders
{
    public class StringPropertyValue : PropertyValue
    {
        string stringValue;

        public StringPropertyValue(string value)
        {
            this.stringValue = value;
        }

        public override void  WriteValueToJs(System.IO.TextWriter writer)
        {
            writer.Write('\'');
            for (int iChar = 0; iChar < this.stringValue.Length; iChar++)
            {
                char ch = this.stringValue[iChar];

                if (ch == '\'')
                {
                    writer.Write('\\');
                }

                writer.Write(ch);
            }
            writer.Write('\'');
        }
    }
}
