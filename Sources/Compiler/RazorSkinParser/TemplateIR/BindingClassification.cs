using System.Collections.Generic;

namespace NScript.RazorSkin.TemplateIR
{
    public enum BindingMode
    {
        OneTime,   // No observable dependencies — evaluate once
        OneWay,    // Has observable dependencies — live updates
        Event      // Event handler (method ref or lambda)
    }

    public enum BindingSourceKind
    {
        DataContext,      // @Model.* references
        TemplateParent,   // @Control.* references
        Mixed             // Expression references both Model and Control
    }

    public class ObservableDependency
    {
        public BindingSourceKind SourceKind { get; set; }
        public string PropertyName { get; set; }
        public string PropertyChain { get; set; } // e.g., "Customer.Address.City"

        public ObservableDependency(BindingSourceKind sourceKind, string propertyName, string propertyChain)
        {
            SourceKind = sourceKind;
            PropertyName = propertyName;
            PropertyChain = propertyChain;
        }
    }

    public class BindingClassification
    {
        public BindingMode Mode { get; set; }
        public BindingSourceKind SourceKind { get; set; }
        public List<ObservableDependency> Dependencies { get; set; } = new List<ObservableDependency>();
        public string CSharpExpression { get; set; } // Original C# expression text
    }
}
