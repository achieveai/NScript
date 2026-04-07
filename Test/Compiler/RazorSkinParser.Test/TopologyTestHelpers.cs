using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Shared IR construction helpers for GraphTopologyBuilderTests and RazorSkinJSTGeneratorTests.
    /// Eliminates duplicate template construction boilerplate across test classes.
    /// </summary>
    internal static class TopologyTestHelpers
    {
        /// <summary>
        /// Creates a SkinTemplateNode with the given children.
        /// Uses "TestTemplate" / "TestModel" as default names.
        /// </summary>
        internal static SkinTemplateNode MakeTemplate(params IRNode[] children)
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel"
            };
            foreach (var child in children)
                template.Children.Add(child);
            return template;
        }

        /// <summary>
        /// Creates an ExpressionBindingNode with DataContext as the default source kind.
        /// </summary>
        internal static ExpressionBindingNode MakeBinding(
            string csharpExpr,
            BindingMode mode,
            ExpressionTarget target,
            string elementId,
            string propertyName)
        {
            return MakeBinding(csharpExpr, mode, BindingSourceKind.DataContext, target, elementId, propertyName);
        }

        /// <summary>
        /// Creates an ExpressionBindingNode with explicit source kind.
        /// </summary>
        internal static ExpressionBindingNode MakeBinding(
            string csharpExpr,
            BindingMode mode,
            BindingSourceKind sourceKind,
            ExpressionTarget target,
            string elementId,
            string propertyName)
        {
            var deps = mode == BindingMode.OneTime
                ? new List<ObservableDependency>()
                : new List<ObservableDependency>
                {
                    new ObservableDependency(sourceKind, propertyName, propertyName)
                };

            return new ExpressionBindingNode
            {
                Target = target,
                ElementId = elementId,
                Classification = new BindingClassification
                {
                    CSharpExpression = csharpExpr,
                    Mode = mode,
                    SourceKind = sourceKind,
                    Dependencies = deps
                }
            };
        }

        /// <summary>
        /// Creates an EventNode.
        /// </summary>
        internal static EventNode MakeEvent(string eventName, string handler, bool isLambda = false)
        {
            return new EventNode
            {
                DomEventName = eventName,
                HandlerExpression = handler,
                IsLambda = isLambda
            };
        }

        /// <summary>
        /// Creates a reactive ConditionalNode with a single-property condition.
        /// </summary>
        internal static ConditionalNode MakeConditional(
            string expression,
            string propName,
            List<IRNode> trueBranch,
            List<IRNode> falseBranch = null)
        {
            var node = new ConditionalNode
            {
                IsReactive = true,
                Condition = new BindingClassification
                {
                    CSharpExpression = expression,
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    Dependencies = new List<ObservableDependency>
                    {
                        new ObservableDependency(BindingSourceKind.DataContext, propName, propName)
                    }
                },
                TrueBranch = trueBranch
            };
            if (falseBranch != null) node.FalseBranch = falseBranch;
            return node;
        }

        /// <summary>
        /// Creates a LoopNode for an observable collection.
        /// </summary>
        internal static LoopNode MakeLoop(
            string collectionExpr,
            string itemVar,
            List<IRNode> itemTemplate,
            bool isObservable = true)
        {
            return new LoopNode
            {
                ItemVariableName = itemVar,
                CollectionExpression = collectionExpr,
                IsObservableCollection = isObservable,
                CollectionSourceKind = BindingSourceKind.DataContext,
                ItemTemplate = itemTemplate
            };
        }
        /// <summary>
        /// Creates an ExpressionBindingNode with a chained property path (LIMIT-005).
        /// E.g., propertyChain="Customer.Address.City", rootPropName="Customer"
        /// </summary>
        internal static ExpressionBindingNode MakeChainBinding(
            string csharpExpr,
            ExpressionTarget target,
            string elementId,
            string rootPropName,
            string propertyChain)
        {
            return new ExpressionBindingNode
            {
                Target = target,
                ElementId = elementId,
                Classification = new BindingClassification
                {
                    CSharpExpression = csharpExpr,
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    Dependencies = new List<ObservableDependency>
                    {
                        new ObservableDependency(BindingSourceKind.DataContext, rootPropName, propertyChain)
                    }
                }
            };
        }
    }
}
