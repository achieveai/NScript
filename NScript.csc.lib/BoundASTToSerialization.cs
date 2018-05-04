
namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class SerializationContext
    {
        public SerializationContext(SemanticModel semanticModel, SymbolSerializer symbolSerializer)
        {
            SemanticModel = semanticModel;
            SymbolSerializer = symbolSerializer;
        }

        public SemanticModel SemanticModel
        { get; }

        public SymbolSerializer SymbolSerializer
        { get; }

        public int Depth
        { get; set; }
    }


    internal class BoundAstToSerialization
        : BoundAstToNotImplemented<SerializationContext, AstBase>
    {
        private readonly Action<string> _write;
        private readonly Action<string, object[]> _writeLine;
        public readonly StringBuilder sb = new StringBuilder();

        public BoundAstToSerialization()
        {
            _write = (s) => sb.Append(s);
            _writeLine = (s, args) => sb.AppendFormat(s + "\r\n", args);
        }

        public MethodBody SerializeMethodBody(BoundStatementList boundStatement, SerializationContext arg)
        {
            var syntax = (MethodDeclarationSyntax)boundStatement.Syntax;
            var location = syntax.Location?.GetMappedLineSpan();
            var method = arg.SemanticModel.GetDeclaredSymbol(syntax);

            var methodBlock = new MethodBody
            {
                FileName = location?.Path,
                MethodId = arg.SymbolSerializer.GetMethodSpecId(method),
                Body = (ParameterBlock)this.Visit(boundStatement, arg)
            };

            return methodBlock;
        }

        public override AstBase VisitFieldEqualsValue(BoundFieldEqualsValue node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitFieldEqualsValue(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPropertyEqualsValue(BoundPropertyEqualsValue node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPropertyEqualsValue(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitParameterEqualsValue(BoundParameterEqualsValue node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitParameterEqualsValue(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitGlobalStatementInitializer(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDeconstructValuePlaceholder(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTupleOperandPlaceholder(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDup(BoundDup node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDup(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPassByCopy(BoundPassByCopy node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPassByCopy(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitBadExpression(BoundBadExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitBadExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitBadStatement(BoundBadStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitBadStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTypeExpression(BoundTypeExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTypeExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTypeOrValueExpression(BoundTypeOrValueExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTypeOrValueExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNamespaceExpression(BoundNamespaceExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNamespaceExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitUnaryOperator(BoundUnaryOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitUnaryOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitIncrementOperator(BoundIncrementOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitIncrementOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAddressOfOperator(BoundAddressOfOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAddressOfOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPointerIndirectionOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPointerElementAccess(BoundPointerElementAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPointerElementAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitRefTypeOperator(BoundRefTypeOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitRefTypeOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMakeRefOperator(BoundMakeRefOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMakeRefOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitRefValueOperator(BoundRefValueOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitRefValueOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitBinaryOperator(BoundBinaryOperator node, SerializationContext arg)
        {
            WriteIndentation(arg, true);
            WriteLine("{0}: SyntaxKind:{1} Operator: {2}", node.Kind, node.Syntax.Kind(), node.OperatorKind);
            AstBase rv = new BinaryExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Left = (ExpressionSer)this.Visit(node.Left, arg),
                Right = (ExpressionSer)this.Visit(node.Right, arg),
            };

            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTupleBinaryOperator(BoundTupleBinaryOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTupleBinaryOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitUserDefinedConditionalLogicalOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitCompoundAssignmentOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAssignmentOperator(BoundAssignmentOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAssignmentOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDeconstructionAssignmentOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNullCoalescingOperator(BoundNullCoalescingOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNullCoalescingOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConditionalOperator(BoundConditionalOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConditionalOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArrayAccess(BoundArrayAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArrayAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArrayLength(BoundArrayLength node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArrayLength(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAwaitExpression(BoundAwaitExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAwaitExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTypeOfOperator(BoundTypeOfOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTypeOfOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMethodDefIndex(BoundMethodDefIndex node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMethodDefIndex(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMaximumMethodDefIndex(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitInstrumentationPayloadRoot(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitModuleVersionId(BoundModuleVersionId node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitModuleVersionId(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitModuleVersionIdString(BoundModuleVersionIdString node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitModuleVersionIdString(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSourceDocumentIndex(BoundSourceDocumentIndex node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSourceDocumentIndex(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMethodInfo(BoundMethodInfo node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMethodInfo(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitFieldInfo(BoundFieldInfo node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitFieldInfo(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDefaultExpression(BoundDefaultExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDefaultExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitIsOperator(BoundIsOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitIsOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAsOperator(BoundAsOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAsOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSizeOfOperator(BoundSizeOfOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSizeOfOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConversion(BoundConversion node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConversion(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArgList(BoundArgList node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArgList(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArgListOperator(BoundArgListOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArgListOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitFixedLocalCollectionInitializer(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSequencePoint(BoundSequencePoint node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            AstBase rv = null;

            if (node.StatementOpt != null)
            { rv = base.Visit(node.StatementOpt, arg); }

            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSequencePointExpression(BoundSequencePointExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSequencePointExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSequencePointWithSpan(BoundSequencePointWithSpan node, SerializationContext arg)
        {
            return null;
        }
        public override AstBase VisitBlock(BoundBlock node, SerializationContext arg)
        {
            // return new StatementListSer
            // {
            //     Location = node.Syntax.GetSerLoc(),
            //     Statements = node.Statements.Select(s => (StatementSer)this.Visit(s, arg)).ToList()
            // };
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            foreach (var statement in node.Statements)
            { this.Visit(statement, arg); }

            arg.Depth--;
            return null;
        }
        public override AstBase VisitScope(BoundScope node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitScope(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitStateMachineScope(BoundStateMachineScope node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitStateMachineScope(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLocalDeclaration(BoundLocalDeclaration node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLocalDeclaration(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMultipleLocalDeclarations(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLocalFunctionStatement(BoundLocalFunctionStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLocalFunctionStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSequence(BoundSequence node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSequence(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNoOpStatement(BoundNoOpStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNoOpStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitReturnStatement(BoundReturnStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            AstBase rv = null;
            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            if (node.ExpressionOpt == null)
            { rv = new ReturnStatement { }; }
            else
            { rv = this.Visit(node.ExpressionOpt, arg); }
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitYieldReturnStatement(BoundYieldReturnStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitYieldReturnStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitYieldBreakStatement(BoundYieldBreakStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitYieldBreakStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitThrowStatement(BoundThrowStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitThrowStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitExpressionStatement(BoundExpressionStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.Visit(node.Expression, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSwitchStatement(BoundSwitchStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSwitchStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSwitchSection(BoundSwitchSection node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSwitchSection(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitSwitchLabel(BoundSwitchLabel node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitSwitchLabel(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitBreakStatement(BoundBreakStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitBreakStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitContinueStatement(BoundContinueStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitContinueStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPatternSwitchStatement(BoundPatternSwitchStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPatternSwitchStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPatternSwitchSection(BoundPatternSwitchSection node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPatternSwitchSection(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPatternSwitchLabel(BoundPatternSwitchLabel node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPatternSwitchLabel(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitIfStatement(BoundIfStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitIfStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDoStatement(BoundDoStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDoStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitWhileStatement(BoundWhileStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitWhileStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitForStatement(BoundForStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitForStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitForEachStatement(BoundForEachStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitForEachStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitForEachDeconstructStep(BoundForEachDeconstructStep node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitForEachDeconstructStep(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitUsingStatement(BoundUsingStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitUsingStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitFixedStatement(BoundFixedStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitFixedStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLockStatement(BoundLockStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLockStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTryStatement(BoundTryStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTryStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitCatchBlock(BoundCatchBlock node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitCatchBlock(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLiteral(BoundLiteral node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1} Val: {2}", node.Kind, node.Syntax.Kind(), node.ConstantValue.GetValueToDisplay());

            var rv = this.GetConstLiteral(node.ConstantValue);
            rv.Location = node.Syntax.Location.GetSerLoc();
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitThisReference(BoundThisReference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitThisReference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPreviousSubmissionReference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitHostObjectMemberReference(BoundHostObjectMemberReference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitHostObjectMemberReference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitBaseReference(BoundBaseReference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitBaseReference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLocal(BoundLocal node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLocal(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPseudoVariable(BoundPseudoVariable node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPseudoVariable(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitRangeVariable(BoundRangeVariable node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitRangeVariable(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitParameter(BoundParameter node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}, ParameterRef: {2}", node.Kind, node.Syntax.Kind(), node.ParameterSymbol);
            var rv = new ParameterReferenceExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Parameter = new ParameterSer
                {
                    Name = node.ParameterSymbol.Name,
                    // Type = node.Type
                }
            };
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLabelStatement(BoundLabelStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLabelStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitGotoStatement(BoundGotoStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitGotoStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLabeledStatement(BoundLabeledStatement node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLabeledStatement(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLabel(BoundLabel node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLabel(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitStatementList(BoundStatementList node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            foreach(var statement in node.Statements)
            { var rv = base.Visit(statement, arg); }

            arg.Depth--;
            return null;
        }
        public override AstBase VisitConditionalGoto(BoundConditionalGoto node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConditionalGoto(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicMemberAccess(BoundDynamicMemberAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicMemberAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicInvocation(BoundDynamicInvocation node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicInvocation(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConditionalAccess(BoundConditionalAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConditionalAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLoweredConditionalAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConditionalReceiver(BoundConditionalReceiver node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConditionalReceiver(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitComplexConditionalReceiver(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitMethodGroup(BoundMethodGroup node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitMethodGroup(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPropertyGroup(BoundPropertyGroup node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPropertyGroup(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitCall(BoundCall node, SerializationContext arg)
        {
            WriteIndentation(arg);
            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());

            WriteIndentation(arg, false);
            WriteLine("{0}, IsVirt:{1} Start Args", node.Method, node.SuppressVirtualCalls);

            foreach (var argNode in node.Arguments)
            { this.Visit(argNode, arg); }

            arg.Depth--;
            return null;
        }
        public override AstBase VisitEventAssignmentOperator(BoundEventAssignmentOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitEventAssignmentOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAttribute(BoundAttribute node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAttribute(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitObjectCreationExpression(BoundObjectCreationExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitObjectCreationExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTupleLiteral(BoundTupleLiteral node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTupleLiteral(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConvertedTupleLiteral(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicObjectCreationExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNoPiaObjectCreationExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitObjectInitializerExpression(BoundObjectInitializerExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitObjectInitializerExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitObjectInitializerMember(BoundObjectInitializerMember node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitObjectInitializerMember(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicObjectInitializerMember(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitCollectionInitializerExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitCollectionElementInitializer(BoundCollectionElementInitializer node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitCollectionElementInitializer(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicCollectionElementInitializer(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitImplicitReceiver(BoundImplicitReceiver node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitImplicitReceiver(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAnonymousObjectCreationExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitAnonymousPropertyDeclaration(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNewT(BoundNewT node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNewT(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDelegateCreationExpression(BoundDelegateCreationExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDelegateCreationExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArrayCreation(BoundArrayCreation node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArrayCreation(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitArrayInitialization(BoundArrayInitialization node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitArrayInitialization(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitStackAllocArrayCreation(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConvertedStackAllocExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitFieldAccess(BoundFieldAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitFieldAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitHoistedFieldAccess(BoundHoistedFieldAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitHoistedFieldAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitPropertyAccess(BoundPropertyAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitPropertyAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitEventAccess(BoundEventAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitEventAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitIndexerAccess(BoundIndexerAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitIndexerAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDynamicIndexerAccess(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitLambda(BoundLambda node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitLambda(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitUnboundLambda(UnboundLambda node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitUnboundLambda(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitQueryClause(BoundQueryClause node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitQueryClause(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitTypeOrInstanceInitializers(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNameOfOperator(BoundNameOfOperator node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNameOfOperator(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitInterpolatedString(BoundInterpolatedString node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitInterpolatedString(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitStringInsert(BoundStringInsert node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitStringInsert(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitIsPatternExpression(BoundIsPatternExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitIsPatternExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDeclarationPattern(BoundDeclarationPattern node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDeclarationPattern(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConstantPattern(BoundConstantPattern node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConstantPattern(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitWildcardPattern(BoundWildcardPattern node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitWildcardPattern(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDiscardExpression(BoundDiscardExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDiscardExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitThrowExpression(BoundThrowExpression node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitThrowExpression(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitOutVariablePendingInference(OutVariablePendingInference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitOutVariablePendingInference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitDeconstructionVariablePendingInference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitOutDeconstructVarPendingInference(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitNonConstructorMethodBody(node, arg);
            arg.Depth--;
            return rv;
        }
        public override AstBase VisitConstructorMethodBody(BoundConstructorMethodBody node, SerializationContext arg)
        {
            WriteIndentation(arg);
            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitConstructorMethodBody(node, arg);
            arg.Depth--;
            return rv;
        }

        private List<StatementSer> GetStatementSers(ImmutableArray<BoundStatement> statements, SerializationContext arg)
            => statements
                .Select(s => (StatementSer)this.Visit(s, arg))
                .Where(_ => _ != null)
                .ToList();

        private AstBase GetConstLiteral(ConstantValue constValue)
        {
            switch (constValue.Discriminator)
            {
                case ConstantValueTypeDiscriminator.Null:
                    return new NullExpression { };
                case ConstantValueTypeDiscriminator.Bad:
                    throw new NotImplementedException();
                case ConstantValueTypeDiscriminator.SByte:
                    return new SByteLiteralExpression
                    { Value = constValue.SByteValue };
                case ConstantValueTypeDiscriminator.Byte:
                    return new ByteLiteralExpression
                    { Value = constValue.ByteValue };
                case ConstantValueTypeDiscriminator.Int16:
                    return new ShortLiteralExpression
                    { Value = constValue.Int16Value } ;
                case ConstantValueTypeDiscriminator.UInt16:
                    return new UShortLiteralExpression
                    { Value = constValue.UInt16Value } ;
                case ConstantValueTypeDiscriminator.Int32:
                    return new IntLiteralExpression
                    { Value = constValue.Int32Value };
                case ConstantValueTypeDiscriminator.UInt32:
                    return new UIntLiteralExpression
                    { Value = constValue.UInt32Value };
                case ConstantValueTypeDiscriminator.Int64:
                    return new LongLiteralExpression
                    { Value = constValue.Int64Value };
                case ConstantValueTypeDiscriminator.UInt64:
                    return new ULongLiteralExpression
                    { Value = constValue.UInt64Value };
                case ConstantValueTypeDiscriminator.Char:
                    return new CharLiteralExpression
                    { Value = constValue.CharValue };
                case ConstantValueTypeDiscriminator.Boolean:
                    return new BoolLiteralExpression
                    { Value = constValue.BooleanValue };
                case ConstantValueTypeDiscriminator.Single:
                    return new FloatLiteralExpression
                    { Value = constValue.SingleValue };
                case ConstantValueTypeDiscriminator.Double:
                    return new DoubleLiteralExpression
                    { Value = constValue.DoubleValue };
                case ConstantValueTypeDiscriminator.String:
                    return new StringLiteralExpression
                    { Value = constValue.StringValue };
                case ConstantValueTypeDiscriminator.Decimal:
                    return new DecimalLiteralExpression
                    { Value = constValue.DecimalValue };
                case ConstantValueTypeDiscriminator.DateTime:
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        private void WriteIndentation(SerializationContext arg, bool shift = true)
        {
            for (int idx = arg.Depth - 1; idx >= 0; idx--)
            { Write("  "); }

            if (shift)
            { arg.Depth++; }
        }

        private void Write(string str)
        { _write(str); }

        private void WriteLine(string fmt, params object[] args)
        { _writeLine(fmt, args); }
    }

    internal class BoundAstToNotImplemented<A, R> : BoundTreeVisitor<A, R>
    {
        public override R DefaultVisit(BoundNode node, A arg)
        { throw new NotImplementedException(); }
        public override R VisitAddressOfOperator(BoundAddressOfOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArgList(BoundArgList node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArgListOperator(BoundArgListOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArrayAccess(BoundArrayAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArrayCreation(BoundArrayCreation node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArrayInitialization(BoundArrayInitialization node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitArrayLength(BoundArrayLength node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAsOperator(BoundAsOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAssignmentOperator(BoundAssignmentOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAttribute(BoundAttribute node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitAwaitExpression(BoundAwaitExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBadExpression(BoundBadExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBadStatement(BoundBadStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBaseReference(BoundBaseReference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBinaryOperator(BoundBinaryOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBlock(BoundBlock node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitBreakStatement(BoundBreakStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitCall(BoundCall node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitCatchBlock(BoundCatchBlock node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitCollectionElementInitializer(BoundCollectionElementInitializer node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConditionalAccess(BoundConditionalAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConditionalGoto(BoundConditionalGoto node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConditionalOperator(BoundConditionalOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConditionalReceiver(BoundConditionalReceiver node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConstantPattern(BoundConstantPattern node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConstructorMethodBody(BoundConstructorMethodBody node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitContinueStatement(BoundContinueStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConversion(BoundConversion node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDeclarationPattern(BoundDeclarationPattern node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDefaultExpression(BoundDefaultExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDelegateCreationExpression(BoundDelegateCreationExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDiscardExpression(BoundDiscardExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDoStatement(BoundDoStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDup(BoundDup node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicInvocation(BoundDynamicInvocation node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicMemberAccess(BoundDynamicMemberAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitEventAccess(BoundEventAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitEventAssignmentOperator(BoundEventAssignmentOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitExpressionStatement(BoundExpressionStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitFieldAccess(BoundFieldAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitFieldEqualsValue(BoundFieldEqualsValue node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitFieldInfo(BoundFieldInfo node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitFixedStatement(BoundFixedStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitForEachDeconstructStep(BoundForEachDeconstructStep node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitForEachStatement(BoundForEachStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitForStatement(BoundForStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitGotoStatement(BoundGotoStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitHoistedFieldAccess(BoundHoistedFieldAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitHostObjectMemberReference(BoundHostObjectMemberReference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitIfStatement(BoundIfStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitImplicitReceiver(BoundImplicitReceiver node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitIncrementOperator(BoundIncrementOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitIndexerAccess(BoundIndexerAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitInterpolatedString(BoundInterpolatedString node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitIsOperator(BoundIsOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitIsPatternExpression(BoundIsPatternExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLabel(BoundLabel node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLabeledStatement(BoundLabeledStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLabelStatement(BoundLabelStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLambda(BoundLambda node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLiteral(BoundLiteral node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLocal(BoundLocal node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLocalDeclaration(BoundLocalDeclaration node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLocalFunctionStatement(BoundLocalFunctionStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLockStatement(BoundLockStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMakeRefOperator(BoundMakeRefOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMethodDefIndex(BoundMethodDefIndex node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMethodGroup(BoundMethodGroup node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMethodInfo(BoundMethodInfo node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitModuleVersionId(BoundModuleVersionId node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitModuleVersionIdString(BoundModuleVersionIdString node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNameOfOperator(BoundNameOfOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNamespaceExpression(BoundNamespaceExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNewT(BoundNewT node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNoOpStatement(BoundNoOpStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitNullCoalescingOperator(BoundNullCoalescingOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitObjectCreationExpression(BoundObjectCreationExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitObjectInitializerExpression(BoundObjectInitializerExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitObjectInitializerMember(BoundObjectInitializerMember node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitOutVariablePendingInference(OutVariablePendingInference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitParameter(BoundParameter node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitParameterEqualsValue(BoundParameterEqualsValue node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPassByCopy(BoundPassByCopy node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPatternSwitchLabel(BoundPatternSwitchLabel node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPatternSwitchSection(BoundPatternSwitchSection node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPatternSwitchStatement(BoundPatternSwitchStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPointerElementAccess(BoundPointerElementAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPropertyAccess(BoundPropertyAccess node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPropertyEqualsValue(BoundPropertyEqualsValue node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPropertyGroup(BoundPropertyGroup node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitPseudoVariable(BoundPseudoVariable node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitQueryClause(BoundQueryClause node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitRangeVariable(BoundRangeVariable node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitRefTypeOperator(BoundRefTypeOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitRefValueOperator(BoundRefValueOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitReturnStatement(BoundReturnStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitScope(BoundScope node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSequence(BoundSequence node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSequencePoint(BoundSequencePoint node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSequencePointExpression(BoundSequencePointExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSequencePointWithSpan(BoundSequencePointWithSpan node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSizeOfOperator(BoundSizeOfOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSourceDocumentIndex(BoundSourceDocumentIndex node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitStateMachineScope(BoundStateMachineScope node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitStatementList(BoundStatementList node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitStringInsert(BoundStringInsert node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSwitchLabel(BoundSwitchLabel node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSwitchSection(BoundSwitchSection node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitSwitchStatement(BoundSwitchStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitThisReference(BoundThisReference node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitThrowExpression(BoundThrowExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitThrowStatement(BoundThrowStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTryStatement(BoundTryStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTupleBinaryOperator(BoundTupleBinaryOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTupleLiteral(BoundTupleLiteral node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTypeExpression(BoundTypeExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTypeOfOperator(BoundTypeOfOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitTypeOrValueExpression(BoundTypeOrValueExpression node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitUnaryOperator(BoundUnaryOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitUnboundLambda(UnboundLambda node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitUsingStatement(BoundUsingStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitWhileStatement(BoundWhileStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitWildcardPattern(BoundWildcardPattern node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitYieldBreakStatement(BoundYieldBreakStatement node, A arg)
            {throw new NotImplementedException(); }
        public override R VisitYieldReturnStatement(BoundYieldReturnStatement node, A arg)
            {throw new NotImplementedException(); }
    }
}