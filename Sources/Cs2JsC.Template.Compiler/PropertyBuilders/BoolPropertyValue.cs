using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Template.Compiler.PropertyBuilders
{
    public class BoolPropertyValue : PropertyValue
    {
        bool value;

        public BoolPropertyValue(bool value)
        {
            this.value = value;
        }

        public override void WriteValueToJs(System.IO.TextWriter writer)
        {
            writer.Write(this.value ? "true" : "false");
        }
    }
}
