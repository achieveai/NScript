//-----------------------------------------------------------------------
// <copyright file="TestGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Plugins
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TestGenerator
    /// </summary>
    public class TestGenerator : IRuntimeConverterPlugin
    {
        /// <summary>
        /// ClrContext for conversoin sessoin.
        /// </summary>
        private ClrContext clrContext;

        /// <summary>
        /// Runtime scope manager for conversion session.
        /// </summary>
        private RuntimeScopeManager scopeManager;

        /// <summary>
        /// Backing field for TestFixtureAttribure.
        /// </summary>
        private TypeReference testFixtureAttribute;

        /// <summary>
        /// Backing field for AsyncTestFixtureAttribute.
        /// </summary>
        private TypeReference asyncTestFixtureAttribute;

        /// <summary>
        /// Backing field for TestSetupAttribute
        /// </summary>
        private TypeReference testSetupAttribute;

        /// <summary>
        /// The test tear down attribute.
        /// </summary>
        private TypeReference testTearDownAttribute;

        /// <summary>
        /// Backing field for TestAttribute
        /// </summary>
        private TypeReference testAttribute;

        /// <summary>
        /// Backing field for TestSetup
        /// </summary>
        private TypeReference testSetup;

        /// <summary>
        /// Backing field for TestSetup
        /// </summary>
        private TypeReference testEnvironment;

        /// <summary>
        /// Backing field for Action
        /// </summary>
        private TypeReference action;

        /// <summary>
        /// Backing field for ModuleMethod
        /// </summary>
        private MethodReference moduleMethod;

        /// <summary>
        /// Backing field for TestMethod
        /// </summary>
        private MethodReference testMethod;

        /// <summary>
        /// Initializes the specified CLR context.
        /// </summary>
        /// <param name="clrContext">The CLR context.</param>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        public void Initialize(
            ClrContext clrContext,
            RuntimeScopeManager runtimeScopeManager)
        {
            this.clrContext = clrContext;
            this.scopeManager = runtimeScopeManager;
        }

        /// <summary>
        /// Gets the methods to emit pass1.
        /// </summary>
        /// <returns>List of methods to include in final JS.</returns>
        public List<MethodReference> GetMethodsToEmitPass1()
        {
            List<MethodReference> rv = new List<MethodReference>();
            foreach (var typeDefinition in clrContext.GetTypeDefinitions())
            {
                if (typeDefinition.CustomAttributes.SelectAttribute(this.TestFixtureAttribute) == null
                    || typeDefinition.HasGenericParameters)
                {
                    continue;
                }

                foreach (var method in typeDefinition.Methods)
                {
                    if (method.HasGenericParameters)
                    { continue; }

                    if (method.CustomAttributes.SelectAttribute(this.TestAttribute) != null
                        || method.CustomAttributes.SelectAttribute(this.TestSetupAttribute) != null)
                    {
                        rv.Add(method);
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Gets the methods to emit pass N.
        /// </summary>
        /// <param name="methodsEmitted">The methods emitted.</param>
        /// <returns></returns>
        public List<MethodReference> GetMethodsToEmitPassN()
        {
            return null;
        }

        /// <summary>
        /// Gets the pre javascript.
        /// </summary>
        /// <returns></returns>
        public List<Statement> GetPreJavascript()
        {
            return null;
        }

        /// <summary>
        /// Gets the post javascript.
        /// </summary>
        /// <returns></returns>
        public List<Statement> GetPostJavascript()
        {
            List<Statement> rv = new List<Statement>();

            foreach (var typeDefinition in clrContext.GetTypeDefinitions())
            {
                if (typeDefinition.CustomAttributes.SelectAttribute(this.TestFixtureAttribute) == null
                    || typeDefinition.HasGenericParameters)
                {
                    continue;
                }

                var moduleMehtodIdentifier = this.scopeManager.Resolve(this.ModuleMethod);
                var testMethodIdentifier = this.scopeManager.Resolve(this.TestMethod);
                IList<IIdentifier> moduleTestSetupIdentifier = null;
                IList<IIdentifier> moduleTestTeardownIdentifier = null;

                TypeConverter typeConverter = this.scopeManager.GetTypeConverter(typeDefinition);
                foreach (var method in typeDefinition.Methods)
                {
                    if (method.HasGenericParameters || !method.IsStatic)
                    { continue; }

                    if (method.CustomAttributes.SelectAttribute(this.TestSetupAttribute) != null)
                    {
                        moduleTestSetupIdentifier = typeConverter.ResolveStaticMember(method);
                    }
                    else if (method.CustomAttributes.SelectAttribute(this.TestTeardownAttribute) != null)
                    {
                        moduleTestTeardownIdentifier = typeConverter.ResolveStaticMember(method);
                    }
                }

                var testEnvironment = new InlineObjectInitializer(null, this.scopeManager.Scope);
                if (moduleTestSetupIdentifier != null)
                {
                    testEnvironment.AddInitializer(
                        "setup",
                        IdentifierExpression.Create(
                            null,
                            this.scopeManager.Scope,
                            moduleTestSetupIdentifier));
                }

                if (moduleTestTeardownIdentifier != null)
                {
                    testEnvironment.AddInitializer(
                        "teardown",
                        IdentifierExpression.Create(
                            null,
                            this.scopeManager.Scope,
                            moduleTestTeardownIdentifier));
                }

                rv.Add(
                    ExpressionStatement.CreateMethodCallExpression(
                        new IdentifierExpression(moduleMehtodIdentifier, this.scopeManager.Scope),
                        new StringLiteralExpression(this.scopeManager.Scope, typeDefinition.FullName),
                        testEnvironment));

                foreach (var method in typeDefinition.Methods)
                {
                    if (method.HasGenericParameters || !method.IsStatic)
                    { continue; }

                    if (method.CustomAttributes.SelectAttribute(this.TestAttribute) != null)
                    {
                        rv.Add(
                            ExpressionStatement.CreateMethodCallExpression(
                                new IdentifierExpression(testMethodIdentifier, this.scopeManager.Scope),
                                new StringLiteralExpression(this.scopeManager.Scope, method.Name),
                                new NumberLiteralExpression(this.scopeManager.Scope, 0),
                                IdentifierExpression.Create(
                                    null,
                                    this.scopeManager.Scope,
                                    typeConverter.ResolveStaticMember(method))));
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Parses the args.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ParseArgs(IList<Tuple<string, string>> args)
        {
        }

        /// <summary>
        /// Gets the test attribute.
        /// </summary>
        private TypeReference TestAttribute
        {
            get
            {
                if (this.testAttribute == null)
                {
                    this.testAttribute =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestAttribute"));
                }

                return this.testAttribute;
            }
        }

        /// <summary>
        /// Gets the test fixture attribute.
        /// </summary>
        private TypeReference TestFixtureAttribute
        {
            get
            {
                if (this.testFixtureAttribute == null)
                {
                    this.testFixtureAttribute =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestFixtureAttribute"));
                }

                return this.testFixtureAttribute;
            }
        }

        /// <summary>
        /// Gets the test fixture attribute.
        /// </summary>
        private TypeReference AsyncTestFixtureAttribute
        {
            get
            {
                if (this.asyncTestFixtureAttribute == null)
                {
                    this.asyncTestFixtureAttribute =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.AsyncTestFixtureAttribute"));
                }

                return this.asyncTestFixtureAttribute;
            }
        }

        /// <summary>
        /// Gets the test setup attribute.
        /// </summary>
        private TypeReference TestSetupAttribute
        {
            get
            {
                if (this.testSetupAttribute == null)
                {
                    this.testSetupAttribute =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestSetupAttribute"));
                }

                return this.testSetupAttribute;
            }
        }

        /// <summary>
        /// Gets the test setup attribute.
        /// </summary>
        private TypeReference TestTeardownAttribute
        {
            get
            {
                if (this.testTearDownAttribute == null)
                {
                    this.testTearDownAttribute =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestTearDownAttribute"));
                }

                return this.testTearDownAttribute;
            }
        }

        /// <summary>
        /// Gets the test setup attribute.
        /// </summary>
        private TypeReference TestSetup
        {
            get
            {
                if (this.testSetup == null)
                {
                    this.testSetup =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestSetup"));
                }

                return this.testSetup;
            }
        }

        /// <summary>
        /// Gets the test environment.
        /// </summary>
        /// <value>
        /// The test environment.
        /// </value>
        private TypeReference TestEnvironment
        {
            get
            {
                if (this.testEnvironment == null)
                {
                    this.testEnvironment =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "SunlightUnit",
                                "SunlightUnit.TestEnvironment"));
                }

                return this.testEnvironment;
            }
        }

        /// <summary>
        /// Gets the test setup attribute.
        /// </summary>
        private TypeReference Action
        {
            get
            {
                if (this.action == null)
                {
                    this.action =
                        this.clrContext.GetTypeDefinition(
                            Tuple.Create(
                                "mscorlib",
                                "System.Action"));
                }

                return this.action;
            }
        }

        /// <summary>
        /// Gets the module method.
        /// </summary>
        private MethodReference ModuleMethod
        {
            get
            {
                if (this.moduleMethod == null)
                {
                    this.moduleMethod = new MethodReference(
                        "Module",
                        this.clrContext.KnownReferences.Void,
                        this.TestSetup);

                    this.moduleMethod.Parameters.Add(
                        new ParameterDefinition(
                            this.clrContext.KnownReferences.String));

                    this.moduleMethod.Parameters.Add(
                        new ParameterDefinition(
                            this.TestEnvironment));
                }

                return this.moduleMethod;
            }
        }

        /// <summary>
        /// Gets the test method.
        /// </summary>
        private MethodReference TestMethod
        {
            get
            {
                if (this.testMethod == null)
                {
                    this.testMethod = new MethodReference(
                        "Test",
                        this.clrContext.KnownReferences.Void,
                        this.TestSetup);

                    this.testMethod.Parameters.Add(
                        new ParameterDefinition(
                            this.clrContext.KnownReferences.String));

                    this.testMethod.Parameters.Add(
                        new ParameterDefinition(
                            this.clrContext.KnownReferences.Int32));

                    this.testMethod.Parameters.Add(
                        new ParameterDefinition(
                            this.Action));
                }

                return this.testMethod;
            }
        }
    }
}