//-----------------------------------------------------------------------
// <copyright file="JSBaseObjectIdentifierManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using NScript.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for JSBaseObjectIdentifierManager
    /// </summary>
    public class JSBaseObjectIdentifierManager
    {
        /// <summary>
        /// Base Identifier scope for static methods.
        /// This scope is used for runtime specific member management and JS
        /// variables e.g. prototype
        /// </summary>
        private readonly IdentifierScope staticBaseIdentifiers;

        /// <summary>
        /// Base Identifier scope for instance methods.
        /// This scope is used for members like constructor or __proto__ etc.
        /// </summary>
        private readonly IdentifierScope instanceBaseIdentifiers;

        /// <summary>
        /// Backing field for ObjectTypeScopeManager.
        /// </summary>
        private readonly TypeScopeManager objectTypeScopeManager;

        /// <summary>
        /// Backing field for TypeTypeScopeManager.
        /// </summary>
        private readonly TypeScopeManager typeTypeScopeManager;

        /// <summary>
        /// Backing store for runtimeScopeManager.
        /// </summary>
        private readonly RuntimeScopeManager runtimeScopeManager;

        /// <summary>
        /// Backing field for Prototype
        /// </summary>
        private IIdentifier prototypeIdentifier;

        /// <summary>
        /// Backing field for Constructor
        /// </summary>
        private IIdentifier constructorIdentifier;

        /// <summary>
        /// Backing field for TypeId
        /// </summary>
        private IIdentifier typeIdIdentifier;

        /// <summary>
        /// Backing field for TypeName
        /// </summary>
        private IIdentifier typeNameIdentifier;

        /// <summary>
        /// Backing field for RegisterNamespace
        /// </summary>
        private IIdentifier registerNamespace;

        /// <summary>
        /// Backing field for RegisterClass.
        /// </summary>
        private IIdentifier registerClass;

        /// <summary>
        /// Backing field for RegisterInterface.
        /// </summary>
        private IIdentifier registerInterface;

        /// <summary>
        /// Backing field for RegisterEnum.
        /// </summary>
        private IIdentifier registerEnum;

        /// <summary>
        /// Backing field for GenericTypeRefInitializer.
        /// </summary>
        private IIdentifier genericTypeRefInitializer;

        /// <summary>
        /// DefaultValueGetter backing field.
        /// </summary>
        private IIdentifier defaultValueGetter;

        /// <summary>
        /// Backing store for converter context.
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSBaseObjectIdentifierManager"/> class.
        /// </summary>
        public JSBaseObjectIdentifierManager(
            RuntimeScopeManager runtimeScopeManager)
        {
            this.context = runtimeScopeManager.Context;
            this.instanceBaseIdentifiers = new IdentifierScope(false);
            this.staticBaseIdentifiers = new IdentifierScope(this.instanceBaseIdentifiers, "staticBaseIdentifiers");
            IdentifierScope objectStaticTypeScope = new IdentifierScope(this.staticBaseIdentifiers, "objectStaticTypeScope");

            this.objectTypeScopeManager = new TypeScopeManager(
                this.context,
                (TypeDefinition)this.context.ClrKnownReferences.Object,
                this.instanceBaseIdentifiers,
                objectStaticTypeScope,
                false);

            this.typeTypeScopeManager = new TypeScopeManager(
                this.context,
                (TypeDefinition)this.context.ClrKnownReferences.TypeType,
                this.staticBaseIdentifiers,
                this.staticBaseIdentifiers,
                false);

            this.runtimeScopeManager = runtimeScopeManager;
        }

        /// <summary>
        /// Gets the static scope.
        /// </summary>
        public IdentifierScope StaticScope
        {
            get
            {
                return this.staticBaseIdentifiers;
            }
        }

        /// <summary>
        /// Gets the instance scope.
        /// </summary>
        public IdentifierScope InstanceScope
        {
            get
            {
                return this.instanceBaseIdentifiers;
            }
        }

        /// <summary>
        /// Gets the objec type scope manager.
        /// </summary>
        public TypeScopeManager ObjecTypeScopeManager
        { get { return this.objectTypeScopeManager; } }

        /// <summary>
        /// Gets the type type scope manager.
        /// </summary>
        public TypeScopeManager TypeTypeScopeManager
        { get { return this.typeTypeScopeManager; } }

        /// <summary>
        /// Gets the prototype.
        /// </summary>
        public IIdentifier Prototype
        {
            get
            {
                if (this.prototypeIdentifier == null)
                {
                    this.prototypeIdentifier =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.PrototypeField);
                }

                return this.prototypeIdentifier;
            }
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        public IIdentifier Constructor
        {
            get
            {
                if (this.constructorIdentifier == null)
                {
                    this.constructorIdentifier =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.ConstructorField);
                }

                return this.constructorIdentifier;
            }
        }

        /// <summary>
        /// Gets the type id.
        /// </summary>
        public IIdentifier TypeId
        {
            get
            {
                if (this.typeIdIdentifier == null)
                {
                    this.typeIdIdentifier =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.TypeIdField);
                }

                return this.typeIdIdentifier;
            }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public IIdentifier TypeName
        {
            get
            {
                if (this.typeNameIdentifier == null)
                {
                    this.typeNameIdentifier =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.TypeNameField);
                }

                return this.typeNameIdentifier;
            }
        }

        /// <summary>
        /// Gets the register namespace.
        /// </summary>
        public IIdentifier RegisterNamespace
        {
            get
            {
                if (this.registerNamespace == null)
                {
                    this.registerNamespace = SimpleIdentifier.CreateScopeIdentifier(
                        this.StaticScope,
                        "registerNamespace",
                        true);
                }

                return this.registerNamespace;
            }
        }

        /// <summary>
        /// Gets the register class.
        /// </summary>
        public IIdentifier RegisterClass
        {
            get
            {
                if (this.registerClass == null)
                {
                    this.registerClass =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.RegisterReferenceTypeMethod);
                }

                return this.registerClass;
            }
        }

        /// <summary>
        /// Gets the register interface.
        /// </summary>
        public IIdentifier RegisterInterface
        {
            get
            {
                if (this.registerInterface == null)
                {
                    this.registerInterface =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.RegisterIntefaceMethod);
                }

                return this.registerInterface;
            }
        }

        /// <summary>
        /// Gets the register enum.
        /// </summary>
        public IIdentifier RegisterEnum
        {
            get
            {
                if (this.registerEnum == null)
                {
                    this.registerEnum =
                        this.runtimeScopeManager.Resolve(
                            this.context.KnownReferences.RegisterEnumMethod);
                }

                return this.registerEnum;
            }
        }

        /// <summary>
        /// Gets the generic type ref initializer.
        /// </summary>
        /// <value>The generic type ref initializer.</value>
        public IIdentifier GenericTypeRefInitializer
        {
            get
            {
                if (this.genericTypeRefInitializer == null)
                {
                    this.genericTypeRefInitializer = SimpleIdentifier.CreateScopeIdentifier(
                        this.staticBaseIdentifiers,
                        "_tri",
                        false);
                }

                return this.genericTypeRefInitializer;
            }
        }

        /// <summary>
        /// Gets the default value getter.
        /// </summary>
        /// <value>The default value getter.</value>
        public IIdentifier DefaultValueGetter
        {
            get
            {
                if (this.defaultValueGetter == null)
                {
                    this.defaultValueGetter = this.runtimeScopeManager.Resolve(
                        this.context.KnownReferences.GetDefaultMethod);
                }

                return this.defaultValueGetter;
            }
        }
    }
}