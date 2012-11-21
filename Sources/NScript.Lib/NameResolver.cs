using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;
using NScript.Lib.AsmDeasm;
using System.IO;

namespace NScript.Lib
{
    public class NameResolver
    {
        #region member variables
        static readonly NameResolver instance = new NameResolver();

        Dictionary<AssemblySignature, Assembly> assembliesHash = new Dictionary<AssemblySignature, Assembly>();
        Dictionary<ClassSignature, Class> classHash = new Dictionary<ClassSignature, Class>();
        Dictionary<FieldSignature, Field> fieldHash = new Dictionary<FieldSignature, Field>();
        Dictionary<MethodSignature, Method> methodHash = new Dictionary<MethodSignature, Method>();
        Dictionary<MethodSignature, Property> propertyHash = new Dictionary<MethodSignature, Property>();
        Dictionary<EventPropertySignature, EventProperty> eventPropertyHash = new Dictionary<EventPropertySignature, EventProperty>();

        HashSet<ClassSignature> importedClasses = new HashSet<ClassSignature>();
        HashSet<ClassSignature> processedClasses = new HashSet<ClassSignature>();

        Dictionary<MethodSignature, JSCompiler> methodSignatures = new Dictionary<MethodSignature, JSCompiler>();

        Dictionary<ClassSignature, string> classNames = new Dictionary<ClassSignature, string>();
        Dictionary<MethodSignature, string> methodNames = new Dictionary<MethodSignature, string>();
        Dictionary<MethodSignature, string> virtualMethodNames = new Dictionary<MethodSignature, string>();
        Dictionary<FieldSignature, string> fieldNames = new Dictionary<FieldSignature, string>();
        #endregion

        #region constructors/Factories
        private NameResolver()
        { }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public static NameResolver Instance
        { get { return NameResolver.instance; } }

        public AssemblySignature CurrentAssembly
        { get; set; }

        public Assembly MainAssembly
        { get; set; }
        #endregion

        #region public functions
        public Assembly ResolveAssembly(AssemblySignature assemblySignature)
        {
            Assembly returnValue = null;
            if (this.assembliesHash.TryGetValue(assemblySignature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        public Class ResolveClass(ClassSignature classSignature)
        {
            Class returnValue = null;
            if (this.classHash.TryGetValue(classSignature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        public Field ResolveField(FieldSignature signature)
        {
            Field returnValue = null;
            if (this.fieldHash.TryGetValue(signature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        public Method ResolveMethod(MethodSignature signature)
        {
            Method returnValue = null;
            if (this.methodHash.TryGetValue(signature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        public Property ResolveProperty(MethodSignature signature)
        {
            Property returnValue = null;
            if (this.propertyHash.TryGetValue(signature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        public EventProperty ResolveEvent(EventPropertySignature signature)
        {
            EventProperty returnValue = null;
            if (this.eventPropertyHash.TryGetValue(signature, out returnValue))
            {
                return returnValue;
            }
            return null;
        }

        internal JSCompiler GetMethodCompiler(MethodSignature methodSignature)
        {
            JSCompiler returnValue;
            if (this.methodSignatures.TryGetValue(methodSignature, out returnValue))
            {
                return returnValue;
            }

            return null;
        }

        internal void AddMethodCompiler(MethodSignature methodSignature, JSCompiler compiler)
        {
            this.methodSignatures.Add(methodSignature, compiler);
        }

        public string GetMemberName(FieldSignature signature)
        {
            return this.fieldNames[signature];
        }

        public string GetMemberName(MethodSignature signature)
        {
            return this.methodNames[signature];
        }

        public string GetVirtualMethodName(MethodSignature signature)
        {
            return this.virtualMethodNames[signature];
        }

        public string GetClassName(ClassSignature signature)
        {
            return this.classNames[signature];
        }

        public void AddAssembly(Assembly assembly)
        {
            this.assembliesHash[assembly.Signature] = assembly;

            foreach (var classObj in assembly.Classes)
            {
                this.AddClass(classObj);
            }

            this.GenerateNames(assembly);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        /// <summary>
        /// Accumilates all the classes, functions and fields within a given assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="classes"></param>
        /// <param name="methods"></param>
        /// <param name="fields"></param>
        private void Accumilate(
            Assembly assembly,
            List<ClassSignature> classes,
            List<MethodSignature> methods,
            List<FieldSignature> fields)
        {
            foreach (var classObject in assembly.Classes)
            {
                classes.Add(classObject.Signature);
                foreach (var method in classObject.Methods)
                {
                    methods.Add(method.Signature);
                }

                foreach (var field in classObject.Fields)
                {
                    fields.Add(field.Signature);
                }
            }
        }

        private void GenerateNames(
            Assembly assembly)
        {
            foreach (var classObject in assembly.Classes)
            {
                this.ProcessClass(classObject.Signature);
            }
        }

        public void ProcessClass(
            ClassSignature signature)
        {
            if (this.processedClasses.Contains(signature))
            {
                return;
            }

            this.processedClasses.Add(signature);

            var classObject = this.classHash[signature];

            // Lets process extended class before
            //
            if (classObject.Extends != null)
            {
                this.ProcessClass(classObject.Extends);
            }

            // Also process intefaces that have been implemented.
            //
            foreach (var inter in classObject.Implements)
            {
                this.ProcessClass(inter);
            }

            bool isImported = false;
            bool ignoreNamespace = false;

            foreach (var attribute in classObject.CustomAttributes)
            {
                if (attribute.AttributeConstructor.Class.Name == "System.ImportedAttribute")
                {
                    isImported = true;
                }
                else if (attribute.AttributeConstructor.Class.Name == "System.IgnoreNamespaceAttribute")
                {
                    ignoreNamespace = true;
                }
            }

            if (classObject.IsDelegate)
            {
                isImported = ignoreNamespace = true;
            }

            if (isImported)
            {
                this.importedClasses.Add(classObject.Signature);
            }

            if (ignoreNamespace)
            {
                this.classNames.Add(
                    classObject.Signature,
                    classObject.Signature.Name.Substring(
                        classObject.Signature.Name.LastIndexOf('.') + 1));
            }
            else
            {
                this.classNames.Add(
                    classObject.Signature,
                    NameResolver.GenerateClassName(classObject.Signature));
            }

            // First let's mark propertyGetters/propertySetters
            //
            foreach (var prop in classObject.Properties)
            {
                bool isIntrinsicProperty = false;
                foreach (var attribute in prop.CustomAttributes)
                {
                    if (attribute.AttributeConstructor.Class.Name == "System.IntrinsicPropertyAttribute")
                    {
                        isIntrinsicProperty = true;
                    }
                }

                if (prop.GetMethod != null)
                {
                    this.methodHash[prop.GetMethod].AttachedAttribute = AttachedMethodAttribute.PropertyGetter;
                    this.methodHash[prop.GetMethod].IsIntrinsic = isIntrinsicProperty;
                }

                if (prop.SetMethod != null)
                {
                    this.methodHash[prop.SetMethod].AttachedAttribute = AttachedMethodAttribute.PropertyGetter;
                    this.methodHash[prop.SetMethod].IsIntrinsic = isIntrinsicProperty;
                }
            }

            foreach (var method in classObject.Methods)
            {
                method.IsImported = isImported;

                if (method.Signature.IsConstructor)
                {
                    // This is a imported class. So the call to constructor is really 
                    // TypeName.call(this, arguments); To allow this, we need to set functionName to TypeName
                    // and set importedFlag on method.
                    //
                    if (isImported)
                    {
                        this.methodNames.Add(
                            method.Signature,
                            this.classNames[classObject.Signature]);
                    }
                    else
                    {
                        this.methodNames.Add(
                            method.Signature,
                            NameResolver.GenerateMemberName(method.Signature));
                    }
                }
                else
                {
                    if (isImported)
                    {
                        string methodName =
                            string.Format("{0}{1}",
                                Char.ToLowerInvariant(method.Signature.Name[0]),
                                method.Signature.Name.Substring(1));

                        this.methodNames[method.Signature] = methodName;
                        this.virtualMethodNames[method.Signature] = methodName;
                    }
                    else
                    {
                        this.methodNames[method.Signature] =
                            NameResolver.GenerateMemberName(method.Signature);

                        this.virtualMethodNames[method.Signature] =
                            NameResolver.GenerateVirtualMethodName(method.Signature);
                    }
                }
            }

            foreach (var field in classObject.Fields)
            {
                this.fieldNames[field.Signature] = NameResolver.GenerateMemberName(field.Signature);
            }
        }

        private static string GenerateVirtualMethodName(MethodSignature signature)
        {
            StringBuilder strBuilder = new StringBuilder(signature.Name.Replace('.', '$'));

            for (int i = 0; i < signature.Arguments.Count; i++)
            {
                strBuilder.AppendFormat("_{0}", signature.Arguments[i].Type.Replace('.', '_'));
            }

            return strBuilder.ToString();
        }

        private static string GenerateMemberName(MethodSignature signature)
        {
            return string.Format("{0}_$_{1}",
                NameResolver.GenerateClassName(signature.Class),
                NameResolver.GenerateVirtualMethodName(signature));
        }

        private static string GenerateClassName(ClassSignature signature)
        {
            return signature.Name.Replace('.', '_');
        }

        private static string GenerateMemberName(FieldSignature signature)
        {
            return string.Format("{0}_$_{1}",
                NameResolver.GenerateClassName(signature.Class),
                signature.Name);
        }

        private void AddClass(Class classObject)
        {
            this.classHash[classObject.Signature] = classObject;

            foreach (var property in classObject.Properties)
            {
                this.propertyHash[property.Signature] = property;
            }

            foreach (var eventProperty in classObject.EventProeprties)
            {
                this.eventPropertyHash[eventProperty.Signature] = eventProperty;
            }

            foreach (var method in classObject.Methods)
            {
                this.methodHash[method.Signature] = method;
            }

            foreach (var field in classObject.Fields)
            {
                this.fieldHash[field.Signature] = field;
            }
        }
        #endregion
    }
}
