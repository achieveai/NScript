using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.MetaData;
using System.IO;
using Cs2JsC.Lib.AsmDeasm;

namespace Cs2JsC.Lib
{
    public class JSCodeGenerator
    {
        #region member variables
        private List<ClassSignature> interfaceOrder = new List<ClassSignature>();
        private List<ClassSignature> classOrder = new List<ClassSignature>();

        private AssemblySignature assemblySignature;
        #endregion

        #region constructors/Factories
        public JSCodeGenerator(Assembly assembly)
        {
            this.Assembly = assembly;
            this.assemblySignature = assembly.Signature;
            this.NewLine = "\r\n";
            this.Indentation = "    ";

            HashSet<ClassSignature> classesVisited = new HashSet<ClassSignature>();
            foreach (var classObject in assembly.Classes)
            {
                this.AddParents(classObject.Signature, classesVisited);
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public string NewLine
        { get; set; }

        public string Indentation
        { get; set; }

        public Assembly Assembly
        { get; private set; }
        #endregion

        #region public functions
        public void Write(TextWriter writer)
        {
            foreach (var item in this.Assembly.Classes)
            {
                this.WriteClass(
                    writer,
                    item.Signature,
                    this.NewLine);
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private void AddParents(ClassSignature classSignature, HashSet<ClassSignature> classesVisited)
        {
            if (classSignature == null ||
                classesVisited.Contains(classSignature) ||
                classSignature.Assembly != this.assemblySignature)
            {
                return;
            }
            classesVisited.Add(classSignature);
            Class classObject = NameResolver.Instance.ResolveClass(classSignature);

            this.AddParents(classObject.Extends, classesVisited);

            foreach (var implementedInterfaces in classObject.Implements)
            {
                this.AddParents(implementedInterfaces, classesVisited);
            }

            if ((classObject.Attributes & TypeAttributes.Interface) == TypeAttributes.Interface)
            {
                this.interfaceOrder.Add(classSignature);
            }
            else
            {
                this.classOrder.Add(classSignature);
            }
        }

        private void WriteClass(TextWriter writer, ClassSignature signature, string prefix)
        {
            Class classObject = NameResolver.Instance.ResolveClass(signature);

            if (classObject.IsDelegate)
            {
                return;
            }

            string className = NameResolver.Instance.GetClassName(signature);

            writer.Write("{0}var {1} = (function {1}_init(){{",
                prefix,
                className);

            string prefix2 = prefix + this.Indentation;

            writer.Write(
                "{0}var retv = function {1}(){{}};",
                prefix2,
                className);

            // TODO: We need to check if we are exporting this class, in that case there should be
            // only one constructor which should become class definition functio.
            //

            // Implement all the functions as normal functions. We will later on use these functions
            // to setup prototype or static functions.
            //
            for (int i = 0; i < classObject.Methods.Count; i++)
            {
                this.WriteMethodImpl(classObject.Methods[i].Signature, writer, prefix2);
            }

            // Now let's go through all the fields and bind them to the type or prototype.
            //
            for (int i = 0; i < classObject.Fields.Count; i++)
            {
                if ((classObject.Fields[i].Attributes & FieldAttributes.Static) == FieldAttributes.Static)
                {
                    writer.Write("{0}retv.{1} = null;",
                        prefix2,
                        NameResolver.Instance.GetMemberName(classObject.Fields[i].Signature));
                }
                else
                {
                    writer.Write("{0}retv.prototype.{1} = null;",
                        prefix2,
                        NameResolver.Instance.GetMemberName(classObject.Fields[i].Signature));
                }
            }

            // Now let's go through all the functions and bind them to the type or prototype.
            //
            for (int i = 0; i < classObject.Methods.Count; i++)
            {
                if ((classObject.Methods[i].Attributes & MethodAttributes.Static) == MethodAttributes.Static)
                {
                    writer.Write("{0}retv.{1} = {1};",
                        prefix2,
                        NameResolver.Instance.GetMemberName(classObject.Methods[i].Signature));
                }
                else
                {
                    writer.Write("{0}retv.prototype.{1} = {1};",
                        prefix2,
                        NameResolver.Instance.GetMemberName(classObject.Methods[i].Signature));
                }
            }

            // Now let's create the classes or interfaces
            //
            if ((classObject.Attributes & TypeAttributes.Interface) == TypeAttributes.Interface)
            {
                writer.Write("{0}retv.createInterface(\"{1}\"",
                    prefix2,
                    NameResolver.Instance.GetClassName(classObject.Signature));

                this.WriteImplementations(writer, classObject.Implements);
                writer.Write(");");
            }
            else
            {
                writer.Write("{0}retv.createClass(\"{1}\", {2}",
                    prefix2,
                    NameResolver.Instance.GetClassName(classObject.Signature),
                    classObject.Extends == null ? "null" : NameResolver.Instance.GetClassName(classObject.Extends));

                if (classObject.Implements.Count > 0)
                {
                    writer.Write(", ");
                    this.WriteImplementations(writer, classObject.Implements);
                }

                writer.Write(");");
            }

            writer.Write("{0}return retv;{1}}})();",
                prefix2,
                prefix);
        }

        private void WriteImplementations(
            TextWriter writer,
            List<ClassSignature> interfaceSignatures)
        {
            for (int i = 0; i < interfaceSignatures.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write(", ");
                }
                writer.Write(NameResolver.Instance.GetClassName(interfaceSignatures[i]));
            }
        }

        private void WriteMethodImpl(
            MethodSignature methodSignature,
            TextWriter writer,
            string prefix)
        {
            writer.Write("{0}function {1}(",
                prefix,
                NameResolver.Instance.GetMemberName(methodSignature));

            this.WriteMethodArgs(methodSignature, writer, prefix + this.Indentation);

            writer.Write("){0}{{", prefix);

            JSCompiler jsCompiler = NameResolver.Instance.GetMethodCompiler(methodSignature);

            string prefix2 = prefix + this.Indentation;
            if (jsCompiler != null)
            {
                jsCompiler.NewLine = this.NewLine;
                jsCompiler.Indentation = this.Indentation;
                jsCompiler.Write(writer, prefix2, this.Indentation);
            }
            else
            {
                // This seems like implementation for interface method. Default implementation
                // calls virtual method.
                //
                writer.Write("{0}return this.{1}(",
                    prefix2,
                    NameResolver.Instance.GetVirtualMethodName(methodSignature));

                this.WriteMethodArgs(methodSignature, writer, prefix2 + this.Indentation);
                writer.Write(");");
            }

            writer.Write("{0}}}", prefix);
        }

        private void WriteMethodArgs(
            MethodSignature methodSignature,
            TextWriter writer,
            string prefix)
        {
            for (int j = 0; j < methodSignature.Arguments.Count; j++)
            {
                if (j > 0)
                {
                    writer.Write(", ");
                }
                writer.Write(methodSignature.Arguments[j].Name);
            }
        }
        #endregion
    }
}
