using System.Collections.Generic;

namespace NScript.RazorSkin.TemplateIR
{
    public abstract class IRNode
    {
        public List<IRNode> Children { get; set; } = new List<IRNode>();
    }

    /// <summary>Root of a skin template IR tree.</summary>
    public class SkinTemplateNode : IRNode
    {
        public string TemplateName { get; set; }
        public string ModelTypeName { get; set; }
        public string ControlTypeName { get; set; }
        public List<string> UsingNamespaces { get; set; } = new List<string>();
        public List<FunctionNode> Functions { get; set; } = new List<FunctionNode>();
    }

    /// <summary>Static HTML content (no bindings).</summary>
    public class HtmlNode : IRNode
    {
        public string HtmlContent { get; set; }
    }

    /// <summary>An @ expression bound to a DOM target (text, attribute, style, CSS class).</summary>
    public class ExpressionBindingNode : IRNode
    {
        public BindingClassification Classification { get; set; }
        public ExpressionTarget Target { get; set; }
        public string ElementId { get; set; } // Part ID if element has id= attribute
    }

    public enum ExpressionTarget
    {
        TextContent,    // @Model.Name as text node
        Attribute,      // value="@Model.X"
        CssClass,       // class="@expr"
        Style           // style="display: @expr"
    }

    public class AttributeExpressionInfo
    {
        public string AttributeName { get; set; }
    }

    /// <summary>Reactive @if / @else block.</summary>
    public class ConditionalNode : IRNode
    {
        public BindingClassification Condition { get; set; }
        public List<IRNode> TrueBranch { get; set; } = new List<IRNode>();
        public List<IRNode> FalseBranch { get; set; } = new List<IRNode>();
        public bool IsReactive { get; set; } // true if condition has observable dependencies
    }

    /// <summary>Reactive @foreach block.</summary>
    public class LoopNode : IRNode
    {
        public string ItemVariableName { get; set; }       // "order" in @foreach(var order in ...)
        public string CollectionExpression { get; set; }   // "Model.Orders"
        public bool IsObservableCollection { get; set; }   // true → incremental DOM updates
        public BindingSourceKind CollectionSourceKind { get; set; }
        public List<IRNode> ItemTemplate { get; set; } = new List<IRNode>(); // Loop body IR
    }

    /// <summary>DOM event handler (onclick, onchange, etc.).</summary>
    public class EventNode : IRNode
    {
        public string DomEventName { get; set; }           // "click", "change", etc.
        public string HandlerExpression { get; set; }      // "Model.OnSubmit" or "(evt) => ..."
        public bool IsLambda { get; set; }                 // true if inline lambda
    }

    /// <summary>Helper function from @functions block.</summary>
    public class FunctionNode : IRNode
    {
        public string FunctionName { get; set; }
        public string CSharpSource { get; set; }
        public bool IsPure { get; set; }                   // true if no Model/Control references
        public List<ObservableDependency> Dependencies { get; set; } = new List<ObservableDependency>();
    }

    /// <summary>Child UIElement declared as PascalCase tag.</summary>
    public class SubControlNode : IRNode
    {
        public string TypeName { get; set; }               // "ListView", "SearchBox"
        public string ResolvedTypeName { get; set; }       // Fully qualified type name
        public string ElementId { get; set; }              // Part ID from id= attribute
        public List<SubControlPropertyBinding> PropertyBindings { get; set; } = new List<SubControlPropertyBinding>();
        public List<EventNode> EventBindings { get; set; } = new List<EventNode>();
    }

    public class SubControlPropertyBinding
    {
        public string PropertyName { get; set; }           // "ObservableList", "Query"
        public BindingClassification Classification { get; set; }
    }
}
