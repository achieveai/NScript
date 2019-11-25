namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;

    public class ConditionalAccessExpression : Expression
    {
        private Expression _receiver;
        private Expression _access;

        public ConditionalAccessExpression(
            ClrContext context,
            Location location,
            Expression receiver,
            Expression access)
            : base(context, location)
        {
            _receiver = receiver;
            _access = access;
        }

        public Expression Receiver
            => _receiver;

        public Expression Access
            => _access;

        public override TypeReference ResultType
        {
            get
            {
                var resultType = _access.ResultType;
                if (resultType.IsValueType)
                {
                    var nullable = this.Context.KnownReferences.NullableType;
                    var rv = new GenericInstanceType(nullable);
                    rv.GenericArguments.Add(resultType);
                    return rv;
                }

                return resultType;
            }
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            _receiver = (Expression)processor.Process(_receiver);
            _access = (Expression)processor.Process(_access);
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("receiver", _receiver);
            serializationInfo.AddValue("access", _access);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ConditionalAccessExpression;
            if (other == null)
            { return false; }

            return this._receiver == other._receiver
                && this._access == other._access;
        }

        public override int GetHashCode()
        {
            return _receiver.GetHashCode() + _access.GetHashCode();
        }

        public class ConditionalReceiver : Expression
        {
            private readonly Expression _nestedExpr;

            public ConditionalReceiver(
                ClrContext context,
                Location location,
                Expression nestedExpr)
                : base(context, location)
            {
                _nestedExpr = nestedExpr;
            }

            public override TypeReference ResultType => _nestedExpr.ResultType;

            public Expression NestedExpression => _nestedExpr;

            public override bool Equals(object obj)
            {
                var other = obj as ConditionalReceiver;
                return other != null
                    && _nestedExpr == other._nestedExpr;
            }

            public override int GetHashCode()
            {
                return nameof(ConditionalReceiver).GetHashCode()
                    | _nestedExpr.GetHashCode();
            }
        }
    }
}
