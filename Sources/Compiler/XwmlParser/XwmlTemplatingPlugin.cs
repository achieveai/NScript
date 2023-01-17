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

    public class XwmlTemplatingPlugin : IMethodConverterPlugin, IRuntimeConverterPlugin
    {
        private string knownCssClasses = string.Empty;

        private KnownTemplateTypes knownTemplateTypes;

        private TypeResolver typeResolver;

        private ParserContext parserContext;

        private CodeGenerator codeGenerator;

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
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
            this.knownTemplateTypes = new KnownTemplateTypes(runtimeScopeManager.Context.ClrKnownReferences);
            this.typeResolver = new TypeResolver(
                runtimeScopeManager,
                runtimeScopeManager.Context.ClrContext);
            this.codeGenerator = new CodeGenerator(runtimeScopeManager, this.knownTemplateTypes);
            this.parserContext = new ParserContext(
                this.knownTemplateTypes,
                this.codeGenerator,
                this.typeResolver,
                this.typeResolver,
                this.knownCssClasses.Split(new char[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries));

            this.parserContext.ConverterContext = runtimeScopeManager.Context;
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
            this.codeGenerator.IterateParsing();
            return new List<MethodReference>();
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        public List<Statement> GetPostJavascript()
        {
            return this.codeGenerator.GetAllTemplateStatements(); ;
        }

        private List<Statement> GetSkinPropertyOverwrite(MethodConverter methodConverter)
        {
            var attr = methodConverter.MethodDefinition.GetPropertyDefinition()
                .CustomAttributes.SelectAttribute(knownTemplateTypes.SkinAttribute);

            var templateName = attr.ConstructorArguments[0].Value as string;

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

            var propName = propertyDefinition.Name;
            var propertyAccessor = new IndexExpression(
                null,
                methodConverter.Scope,
                methodConverter.ResolveThis(methodConverter.Scope, null),
                new IdentifierExpression(
                    methodConverter.RuntimeManager.Resolve(
                        methodConverter.MethodDefinition.GetPropertyDefinition()),
                    methodConverter.Scope));

            if (methodConverter.MethodDefinition.IsGetter)
            {
                return new List<Statement>
                {
                    new ExpressionStatement(
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
                            new List<Expression>
                            {
                                // prop = value
                                new BinaryExpression(
                                    null,
                                    methodConverter.Scope,
                                    BinaryOperator.Assignment,
                                    propertyAccessor,
                                    value),
                                // this.FirePropertyChanged("propName")
                                new MethodCallExpression(
                                    null,
                                    methodConverter.Scope,
                                    new IndexExpression(
                                        null,
                                        methodConverter.Scope,
                                        methodConverter.ResolveThis(methodConverter.Scope, null),
                                        new IdentifierExpression(
                                            methodConverter.RuntimeManager.Resolve(
                                                knownTemplateTypes.FirePropertyChangedMethodReference),
                                            methodConverter.Scope)),
                                    new StringLiteralExpression(methodConverter.Scope, propName))
                            }
                            .ConvertAll(expr => (Statement)new ExpressionStatement(
                                null,
                                methodConverter.Scope,
                                expr))),
                        null)
                };
            }
        }
    }
}
