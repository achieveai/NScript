using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;

namespace NScript.Lib.ILParser
{
    public class ClassInfo : ScopeInfo
    {
        #region member variables
        ILType type;

        List<FieldInfo> fields = new List<FieldInfo>();
        List<MethodInfo> methods = new List<MethodInfo>();
        List<PropertyInfo> properties = new List<PropertyInfo>();
        List<ClassInfo> innerClasses = new List<ClassInfo>();
        // List<EventInfo> events = new List<EventInfo>();
        #endregion

        #region constructors/Factories
        public ClassInfo(
            AssemblyInfo assembly,
            ClassSignature outerClass,
            Scope scope,
            bool parseMetadataOnly)
            : base(scope)
        {
            this.type = ILType.ParseFromHeader(this.Scope.Header);
            this.ParentAssembly = assembly;

            this.Class = new Class(
                this.ParentAssembly.Assembly,
                new ClassSignature(
                    (outerClass != null ? outerClass.Name + "/" : string.Empty) + this.type.Name,
                    this.ParentAssembly.Assembly.Signature,
                    this.type.GenericParameters == null ? 0 : this.type.GenericParameters.Count),
                this.type.Attributes,
                string.IsNullOrEmpty(this.type.ExtendsClassName) ? null : ParseUtils.ParseTypeSignature((StringFragment)this.type.ExtendsClassName),
                AttributeParser.GetAttributes(this.Scope.ScopedLines, 0));

            for (int i = 0; i < this.type.Implements.Count; i++)
            {
                this.Class.Implements.Add(ParseUtils.ParseTypeSignature((StringFragment)this.type.Implements[i]));
            }

            foreach (var innerScope in scope.NestedScopes)
            {
                if (innerScope.ScopeType == ScopeType.Method)
                {
                    this.methods.Add(
                        new MethodInfo(
                            this,
                            innerScope,
                            parseMetadataOnly,
                            this.type.GenericParameters));
                }
                else if (innerScope.ScopeType == ScopeType.Field)
                {
                    this.fields.Add(
                        new FieldInfo(this, innerScope));
                }
                else if (innerScope.ScopeType == ScopeType.Class)
                {
                    this.innerClasses.Add(
                        new ClassInfo(
                            ParentAssembly,
                            this.Class.Signature,
                            innerScope,
                            parseMetadataOnly));
                }
                else if (innerScope.ScopeType == ScopeType.Property)
                {
                    this.properties.Add(
                        new PropertyInfo(
                            this,
                            innerScope,
                            this.type.GenericParameters));
                }
                else
                {
                    Console.WriteLine("Not adding {0}", innerScope.Header);
                }
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public AssemblyInfo ParentAssembly
        {
            get;
            private set;
        }

        public Class Class
        { get; private set; }

        public List<FieldInfo> Fields
        {
            get { return this.fields; }
        }

        public List<MethodInfo> Methods
        {
            get { return this.methods; }
        }

        public List<PropertyInfo> Properties
        { get { return this.properties; } }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
