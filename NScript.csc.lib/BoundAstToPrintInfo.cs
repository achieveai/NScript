
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

    internal class BoundAstToPrintInfo
        : BoundAstToNotImplemented<SerializationContext, AstBase>
    {
        private readonly Action<string> _write;
        private readonly Action<string, object[]> _writeLine;
        public readonly StringBuilder sb = new StringBuilder();

        public BoundAstToPrintInfo()
        {
            _write = (s) => sb.Append(s);
            _writeLine = (s, args) => sb.AppendFormat(s + "\r\n", args);
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
            var rv = new UnaryExpression
            {
                Expression = this.Visit(node.Operand, arg) as ExpressionSer,
                Operator = (int)node.OperatorKind,
                Location = node.Syntax.GetSerLoc(),
                IsLifted = node.ResultConversion.IsBoxing
            };
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
            var rv = new BinaryExpression
            {
                Left = this.Visit(node.Left, arg) as ExpressionSer,
                Right = this.Visit(node.Right, arg) as ExpressionSer,
                IsLifted = node.LeftConversion.IsBoxing,
                Operator = (int)node.Operator.Kind,
                Location = node.Syntax.GetSerLoc()
            };

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
            switch (node.Syntax.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.Block:
                    foreach (var statement in node.Statements)
                    { this.Visit(statement, arg); }
                    break;
                default:
                    throw new NotImplementedException();
            }

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

            WriteLine("{0}: SyntaxKind:{1} Name: {2}", node.Kind, node.Syntax.Kind(), node.LocalSymbol);

            AstBase rv = null;
            if (node.InitializerOpt != null)
            { rv = this.Visit(node.InitializerOpt, arg); }

            arg.Depth--;
            return rv;
        }

        public override AstBase VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = new StatementListSer
            {
                Statements = node
                    .LocalDeclarations
                    .Select(_ =>
                    {
                        var decl = this.Visit(_, arg);
                        return (StatementSer)(
                            new StatementExpressionSer
                            {
                                Expression = decl as ExpressionSer
                            });
                    })
                    .ToList()
            };
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
            AstBase rv = null;
            foreach (var sideEffect in node.SideEffects)
            {
                 base.Visit(sideEffect, arg);
            }
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
/*
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
*/
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
            AstBase rv = new ForStatement
            {
                Initializer = this.Visit(node.Initializer, arg) as StatementSer,
                Condition = this.Visit(node.Condition, arg) as ExpressionSer,
                Iterator = this.Visit(node.Increment, arg) as StatementSer,
                Location = node.Syntax.GetSerLoc(),
                Loop = this.Visit(node.Body, arg) as StatementSer
            };
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

            var rv = BoundAstToAstBase.GetConstLiteral(node.ConstantValue);
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
            var rv = new LocalVariableRefExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                LocalVariable = new LocalVariableSer
                {
                    Name = node.LocalSymbol.Name,
                    // Type = node.Type
                }
            };
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

            WriteLine("{0}: SyntaxKind:{1}, ParameterRef: {2}", node.Kind, node.Syntax.Kind(), node.ParameterSymbol.Name);
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

        /*
        public override AstBase VisitWildcardPattern(BoundWildcardPattern node, SerializationContext arg)
        {
            for (int idx = arg.Depth++ - 1; idx >= 0; idx--)
            { Write("  "); }

            WriteLine("{0}: SyntaxKind:{1}", node.Kind, node.Syntax.Kind());
            var rv = base.VisitWildcardPattern(node, arg);
            arg.Depth--;
            return rv;
        }
        */

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
}