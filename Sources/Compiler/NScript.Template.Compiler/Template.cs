using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using NScript.Template.Compiler.Attributes;
using NScript.PELoader.Reflection;
using NScript.PELoader.Helpers;

namespace NScript.Template.Compiler
{
    public class Template
    {
        #region member variables
        private static TypeReference TemplateReference =
            new TypeReference(KnowTypeReferences.TemplateSignature);
        private static TypeReference TemplateInstanceReference =
            new TypeReference(KnowTypeReferences.TemplateInstanceSignature);
        private static TypeReference UIObjectArrayReference =
            new TypeReference(
                new TypeReference(KnownTypeSignatures.ArraySignature),
                    new TypeReference(KnowTypeReferences.UIObjectSignature));

        private static MethodReference RegisterChildFunction =
            new MethodReference(
                "RegisterChildById",
                false,
                false,
                null,
                new TypeReferenceBase[]
                {
                    new TypeReference(KnownTypeSignatures.StringTypeSignature),
                    new TypeReference(KnowTypeReferences.UIObjectSignature),
                },
                null,
                new TypeReference(KnowTypeReferences.TemplateInstanceSignature));

        List<ElementInfo> elementInfos = new List<ElementInfo>();
        Dictionary<string, string> knownIdMappings = new Dictionary<string,string>();
        Html templateContext;
        TypeReference dataContextType;
        TypeReference templatedControlType;
        string htmlStr;
        string templateId;
        #endregion

        #region constructors/Factories
        internal Template(
            string htmlStr,
            string templateId,
            Html templateContext,
            TypeReference dataContextType,
            TypeReference templatedControlType,
            List<ElementInfo> elementInfos,
            Dictionary<string,string> knownIdMappings)
        {
            this.htmlStr = htmlStr;
            this.templateId = templateId;
            this.templateContext = templateContext;
            this.dataContextType = dataContextType;
            this.templatedControlType = templatedControlType;
            this.elementInfos = elementInfos ?? new List<ElementInfo>();
            this.knownIdMappings = knownIdMappings ?? new Dictionary<string, string>();
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public List<ElementInfo> TemplateElements
        { get { return this.elementInfos; } }

        public Dictionary<string, string> KnownIdMappings
        { get { return this.knownIdMappings; } }

        public string TemplateId
        {
            get
            {
                return this.templateId;
            }
        }
        #endregion

        #region public functions
        /// <summary>
        /// Write JS function that will initialize the templateInstance.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteTemplateInitializationFunction(System.IO.StreamWriter writer)
        {
            // Let's create htmlHolder, function and htmlHolder initializer
            //
            writer.WriteLine("var _html = null;");
            writer.Write(@"function tif(){ if (!_html) { _html = $(window.document.createElement(""div"")); _html.html(""");
            writer.Write(this.htmlStr);
            writer.WriteLine(@""");}");

            writer.WriteLine(@"var tr = _html.clone();");

            writer.WriteLine("var av=[];");

            for (int iElementInfo = 0; iElementInfo < this.TemplateElements.Count; iElementInfo++)
            {
                string elementId = string.Format("av[{0}]", iElementInfo);
                this.TemplateElements[iElementInfo].WriteToJs(
                    "tr",
                    elementId,
                    this.templateContext.StyleMapping,
                    writer);

                // Add this element to a panel, if necessary
                //
                ElementInfo parentPanelInfo = this.TemplateElements[iElementInfo].ParentPanel;
                if (parentPanelInfo != null)
                {
                    // Remove the UIObject from the DOM and add it to the panel
                    //
                    writer.WriteLine(elementId + ".get_attachedElement().remove();");
                    string parentPanelId = String.Format("av[{0}]", this.TemplateElements.IndexOf(parentPanelInfo));
                    writer.WriteLine(parentPanelId + ".get_children().add(" + elementId + ");");
                }
            }

            writer.WriteLine(
                "var ti=new {0}(tr,av);",
                TypeManager.GetTypeReferenceId(Template.TemplateInstanceReference));

            for (int iElementInfo = 0; iElementInfo < this.TemplateElements.Count; iElementInfo++)
            {
                if (this.TemplateElements[iElementInfo].KnownId != null)
                {
                    writer.WriteLine("ti.{0}(\"{1}\",av[{2}]);",
                        Template.RegisterChildFunction,
                        this.TemplateElements[iElementInfo].KnownId,
                        iElementInfo);
                }
            }

            writer.WriteLine("return ti;};");

            writer.WriteLine("return new {0}(\"{1}\", tif);",
                TypeManager.GetTypeReferenceId(Template.TemplateReference),
                this.templateId);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
