
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