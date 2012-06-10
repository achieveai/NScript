using System.Collections.Generic;
using System.IO;

namespace Cs2JsC.Template.Compiler
{
    public static class Writer
    {
        public static void WriteJs(List<Html> parsedTemplates, string fileNameToWriteTo)
        {
            // // We need to generate a unique function for the template as it is posible to have multiple template files
            // //
            // string functionName = "__" + Path.GetFileName(fileNameToWriteTo).Replace('.', '_');
            // using (var writer = new StreamWriter(fileNameToWriteTo))
            // {
            //     writer.WriteLine("var " + functionName + "= function(){");
            //     TypeManager.WriteReferenceInitialization(writer);

            //     for (int iTemplate = 0; iTemplate < parsedTemplates.Count; iTemplate++)
            //     {
            //         parsedTemplates[iTemplate].WriteJs(writer);
            //     }

            //     writer.WriteLine("}();");
            // }
        }

        public static void WriteCss(List<Html> parsedTemplates, string fileNameToWriteTo)
        {
            using (var writer = new StreamWriter(fileNameToWriteTo))
            {
                for (int iTemplate = 0; iTemplate < parsedTemplates.Count; iTemplate++)
                {
                    parsedTemplates[iTemplate].WriteCss(writer);
                }
            }
        }

    }
}
