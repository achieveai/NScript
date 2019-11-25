namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;

    public class LocalMethodStatement : Statement
    {
        private readonly LocalFunctionVariable _functionVariable;
        private ParameterBlock _block;

        public LocalMethodStatement(
            ClrContext context,
            Location location,
            LocalFunctionVariable functionVariable,
            ParameterBlock block)
            : base(context, location)
        {
            _block = block;
            _functionVariable = functionVariable;
        }

        public LocalFunctionVariable LocalFunctionVariable
            => _functionVariable;

        public ParameterBlock MethodBlock
            => _block;

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("block", _block);
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            _block = (ParameterBlock)processor.Process(_block);
        }
    }
}
