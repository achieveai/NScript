using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Template.Compiler
{
    class HtmlContext
    {
        public StringBuilder HtmlString = new StringBuilder();
        public HashSet<string> ids = new HashSet<string>();
        public int TagsDepth = 0;

        public string GenerateId()
        {
            return string.Format("_tmp_{0}", ids.Count);
        }
    }
}
