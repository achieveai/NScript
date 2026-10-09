namespace XwmlParser
{
    using JavaScriptEngineSwitcher.Core;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class XwmlTemplatingPlugin : IMethodConverterPlugin, IRuntimeConverterPlugin
    {
        private string knownCssClasses = string.Empty;

        private KnownTemplateTypes knownTemplateTypes;

        private TypeResolver typeResolver;

        private ParserContext parserContext;

        private CodeGenerator codeGenerator;

        // Probe.XwmlInit: Stopwatch ticks per phase of this build. Overwrite reads the
        // template documents (and the sheets they link), Parse parses the queued templates,
        // Gen compresses CSS names and generates the template code.
        private long probeInitTicks;
        private long probeOverwriteTicks;
        private long probeParseTicks;

        public ParserContext ParserContext
        { get { return this.parserContext; } }

        public TypeResolver TypeResolver
        { get { return this.typeResolver; } }

        public CodeGenerator CodeGenerator
        { get { return this.codeGenerator; } }

        public IntrestLevel GetInterestLevel(
            MethodDefinition methodDefinition,
            ConverterContext converterContext)
        {
            PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();
            if (propertyDefinition != null
                && propertyDefinition.SetMethod == null
                && propertyDefinition.CustomAttributes != null
                && propertyDefinition.CustomAttributes.SelectAttribute(
                    this.knownTemplateTypes.SkinAttribute) != null)
            {
                return IntrestLevel.Overwrite;
            }
            else if (propertyDefinition?.CustomAttributes?.SelectAttribute(
                this.knownTemplateTypes.AutoFireAttribute) != null)
            {
                return IntrestLevel.Overwrite;
            }

            else return IntrestLevel.None;
        }

        public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetEncapsulationStatements(MethodConverter methodConverter, List<Statement> methodStatments)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetOverwrite(MethodConverter methodConverter)
        {
            long start = Stopwatch.GetTimestamp();
            try
            {
                return this.GetOverwriteCore(methodConverter);
            }
            finally
            {
                this.probeOverwriteTicks += Stopwatch.GetTimestamp() - start;
            }
        }

        private List<Statement> GetOverwriteCore(MethodConverter methodConverter)
        {
            var attr = methodConverter.MethodDefinition.GetPropertyDefinition()
                .CustomAttributes.SelectAttribute(this.knownTemplateTypes.SkinAttribute)
                ?? methodConverter.MethodDefinition.GetPropertyDefinition()
                    .CustomAttributes.SelectAttribute(this.knownTemplateTypes.AutoFireAttribute);

            return attr.AttributeType.IsSame(this.knownTemplateTypes.AutoFireAttribute)
                ? GetAutoFirePropertyOverwrite(methodConverter)
                : GetSkinPropertyOverwrite(methodConverter);
        }

        public void Initialize(NScript.CLR.ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            long start = Stopwatch.GetTimestamp();
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
            this.knownTemplateTypes = new KnownTemplateTypes(runtimeScopeManager.Context.ClrKnownReferences);
            this.typeResolver = new TypeResolver(
                runtimeScopeManager,
                runtimeScopeManager.Context.ClrContext);
            this.codeGenerator = new CodeGenerator(runtimeScopeManager, this.knownTemplateTypes);
            var cssClassEntries = this.knownCssClasses.Split(new char[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            this.parserContext = new ParserContext(
                this.knownTemplateTypes,
                this.codeGenerator,
                this.typeResolver,
                this.typeResolver,
                cssClassEntries);

            this.parserContext.ConverterContext = runtimeScopeManager.Context;

            if (CompilerLog.IsEnabled)
            {
                CompilerLog.ForComponent("XwmlParser").Information(
                    "XwmlParser.Initialized KnownCssClassCount={Count}",
                    cssClassEntries.Length);
            }

            this.probeInitTicks = Stopwatch.GetTimestamp() - start;
            this.probeOverwriteTicks = 0;
            this.probeParseTicks = 0;
        }

        public void ParseArgs(IList<Tuple<string, string>> args)
        {
            if (args == null) return;

            foreach (var tupl in args)
            {
                if (tupl.Item1 == "KnownCssClasses")
                {
                    this.knownCssClasses = tupl.Item2;
                }
            }
        }

        public List<MethodReference> GetMethodsToEmitPass1()
        {
            return this.GetMethodsToEmitPassN();
        }

        public List<MethodReference> GetMethodsToEmitPassN()
        {
            long start = Stopwatch.GetTimestamp();
            this.codeGenerator.IterateParsing();
            this.probeParseTicks += Stopwatch.GetTimestamp() - start;
            return new List<MethodReference>();
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        public List<Statement> GetPostJavascript()
        {
            long start = Stopwatch.GetTimestamp();
            var statements = this.codeGenerator.GetAllTemplateStatements();
            long genTicks = Stopwatch.GetTimestamp() - start;

            static long Ms(long ticks) => ticks * 1000 / Stopwatch.Frequency;
            CompilerLog.ForComponent("XwmlParser").Information(
                "Probe.XwmlInit TotalMs={TotalMs} InitMs={InitMs} OverwriteMs={OverwriteMs} ParseMs={ParseMs} GenMs={GenMs} Templates={Templates} TemplatesParsed={TemplatesParsed} StyleSheets={StyleSheets} Hits={Hits} Misses={Misses}",
                Ms(this.probeInitTicks + this.probeOverwriteTicks + this.probeParseTicks + genTicks),
                Ms(this.probeInitTicks),
                Ms(this.probeOverwriteTicks),
                Ms(this.probeParseTicks),
                Ms(genTicks),
                this.codeGenerator.ProbeDocuments,
                this.codeGenerator.ProbeTemplatesParsed,
                this.codeGenerator.ProbeStyleSheets,
                0,
                this.codeGenerator.ProbeDocuments);
            return statements;
        }

        private List<Statement> GetSkinPropertyOverwrite(MethodConverter methodConverter)
        {
            var attr = methodConverter.MethodDefinition.GetPropertyDefinition()
                .CustomAttributes.SelectAttribute(knownTemplateTypes.SkinAttribute);

            var templateName = attr.ConstructorArguments[0].Value as string;

            if (CompilerLog.IsEnabled)
            {
                CompilerLog.ForComponent("XwmlParser").Information(
                    "XwmlParser.Template {TemplateName}", templateName);
            }

            try
            {
                return new List<Statement>()
                {
                    new ReturnStatement(
                        null,
                        methodConverter.Scope,
                        new MethodCallExpression(
                            null,
                            methodConverter.Scope,
                            new IdentifierExpression(
                                this.codeGenerator.GetTemplateGetterIdentifier(templateName),
                                methodConverter.Scope)))
                };
            }
            catch (ConverterLocationException ex)
            {
                this.codeGenerator.ParserContext.ConverterContext.AddError(
                    ex.Location,
                    ex.Message,
                    false);
            }
            catch(ApplicationException ex)
            {
                this.codeGenerator.ParserContext.ConverterContext.AddError(
                    null,
                    ex.Message,
                    false);
            }

            return null;
 
        }

        private List<Statement> GetAutoFirePropertyOverwrite(MethodConverter methodConverter)
        {
            var propertyDefinition = methodConverter.MethodDefinition.GetPropertyDefinition();

            var atrr = propertyDefinition.CustomAttributes.SelectAttribute(
                knownTemplateTypes.AutoFireAttribute);

            if (!propertyDefinition.DeclaringType.ImplementsInterface(
                    this.knownTemplateTypes.ObservableInterface))
            {
                methodConverter.RuntimeManager.Context.AddError(
                    null,
                    $"Autofire only valid on classes implementing {knownTemplateTypes.ObservableInterface.FullName}",
                    false);

                return null;
            }
            else if (propertyDefinition.IsStatic())
            {
                methodConverter.RuntimeManager.Context.AddError(
                    null,
                    $"Autofire only valid on instance properties of {knownTemplateTypes.ObservableInterface.FullName}",
                    false);

                return null;
            }

            var propName = propertyDefinition.Name;
            var propertyAccessor = new IndexExpression(
                null,
                methodConverter.Scope,
                methodConverter.ResolveThis(methodConverter.Scope, null),
                new IdentifierExpression(
                    methodConverter.RuntimeManager.Resolve(
                        MethodConverter.GetBackingField(
                            propertyDefinition,
                            methodConverter.ClrKnownReferences)),
                    methodConverter.Scope));

            if (methodConverter.MethodDefinition.IsGetter)
            {
                return new List<Statement>
                {
                    new ReturnStatement(
                        null,
                        methodConverter.Scope,
                        propertyAccessor)
                };
            }
            else
            {
                // value provided to the setter
                var value = new IdentifierExpression(
                    methodConverter.ResolveArgument(
                        methodConverter.MethodDefinition.Parameters[0].Name),
                    methodConverter.Scope);

                var allPropertiesToFire = new List<string> { propertyDefinition.Name };

                if (atrr.HasConstructorArguments)
                {
                    foreach (var arg in atrr.ConstructorArguments)
                    {
                        var val = ((CustomAttributeArgument[])arg.Value)
                            .Select(arg => arg.Value as string);
                        allPropertiesToFire.AddRange(val);
                    }

                    allPropertiesToFire = allPropertiesToFire.Distinct().ToList();
                }

                MethodCallExpression GetMethodCallFor(string propName)
                    => 
                        methodConverter.RuntimeManager.ImplementInstanceAsStatic
                        ? new(
                            null,
                            methodConverter.Scope,
                            IdentifierExpression.Create(
                                null,
                                methodConverter.Scope,
                                methodConverter.ResolveStaticMember(
                                    knownTemplateTypes.FirePropertyChangedMethodReference)),
                            methodConverter.ResolveThis(methodConverter.Scope, null),
                            new StringLiteralExpression(methodConverter.Scope, propName))
                        : new(
                            null,
                            methodConverter.Scope,
                            new IndexExpression(
                                null,
                                methodConverter.Scope,
                                methodConverter.ResolveThis(methodConverter.Scope, null),
                                methodConverter.ResolveVirtualMethod(
                                    knownTemplateTypes.FirePropertyChangedMethodReference,
                                    methodConverter.Scope)),
                            new StringLiteralExpression(methodConverter.Scope, propName));

                return new List<Statement>
                {
                    new IfBlockStatement(
                        null,
                        methodConverter.Scope,
                        // prop == value
                        condition: new BinaryExpression(
                            null,
                            methodConverter.Scope,
                            BinaryOperator.StrictNotEquals,
                            propertyAccessor,
                            value),
                        trueBlock: new ScopeBlock(
                            null,
                            methodConverter.Scope,
                            statements: (new List<Expression>
                                {
                                    // prop = value
                                    new BinaryExpression(
                                        null,
                                        methodConverter.Scope,
                                        BinaryOperator.Assignment,
                                        propertyAccessor,
                                        value)
                                }
                                .Concat(allPropertiesToFire.Select(GetMethodCallFor))
                                .ToList()
                                .ConvertAll(expr => (Statement)new ExpressionStatement(
                                    null,
                                    methodConverter.Scope,
                                    expr)))),
                        falseBlock: null)
                };
            }
        }
    }
}
