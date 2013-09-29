namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    class XwmlTemplatingPlugin : IMethodConverterPlugin, IRuntimeConverterPlugin
    {
        private KnownTemplateTypes knownTemplateTypes;

        private TypeResolver typeResolver;

        private ParserContext parserContext;

        private CodeGenerator codeGenerator;

        private CodeGenerator GetCodeGenerator(MethodConverter methodConverter)
        {
            if (this.codeGenerator == null)
            {
                this.codeGenerator = new CodeGenerator(methodConverter.RuntimeManager);
            }

            return this.codeGenerator;
        }

        public IntrestLevel GetInterestLevel(
            MethodDefinition methodDefinition,
            ConverterContext converterContext)
        {
            var skinAttribute = this.knownTemplateTypes.SkinAttribute;
            PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();
            if (propertyDefinition != null
                && propertyDefinition.SetMethod == null
                && propertyDefinition.CustomAttributes != null
                && propertyDefinition.CustomAttributes.SelectAttribute(
                    this.knownTemplateTypes.SkinAttribute) != null)
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
                .CustomAttributes.SelectAttribute(this.knownTemplateTypes.SkinAttribute);

            var templateName = attr.ConstructorArguments[0].Value as string;

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

        public void Initialize(NScript.CLR.ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            this.knownTemplateTypes = new KnownTemplateTypes(runtimeScopeManager.Context.ClrKnownReferences);
            this.typeResolver = new TypeResolver(runtimeScopeManager.Context.ClrContext);
            this.codeGenerator = new CodeGenerator(runtimeScopeManager);
            this.parserContext = new ParserContext(
                this.knownTemplateTypes,
                this.codeGenerator,
                this.typeResolver);

            this.parserContext.ConverterContext = runtimeScopeManager.Context;
        }

        public void ParseArgs(IList<Tuple<string, string>> args)
        {
        }

        public List<MethodReference> GetMethodsToEmitPass1()
        {
            return new List<MethodReference>();
        }

        public List<MethodReference> GetMethodsToEmitPassN()
        {
            return new List<MethodReference>();
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        public List<Statement> GetPostJavascript()
        {
            return new List<Statement>();
        }
    }
}
