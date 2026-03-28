namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    /// <summary>
    /// Node type constants for the reactive binding graph.
    /// Values are used as array indices in the static descriptor's nodeTypes array.
    /// </summary>
    public static class GraphNodeType
    {
        public const int Source = 0;
        public const int Property = 1;
        public const int Computed = 2;
        public const int DomTarget = 3;
        public const int EventBinding = 4;
        public const int Gate = 5;
        public const int CollectionManager = 6;
        public const int TypeGuard = 7;
    }

    /// <summary>
    /// Subscription mode for the graph.
    /// </summary>
    public static class GraphSubscribeMode
    {
        public const int PerProperty = 0;
        public const int AllProperties = 1;
    }

    /// <summary>
    /// Source slot indices.
    /// </summary>
    public static class GraphSourceSlot
    {
        public const int DataContext = 0;
        public const int TemplateParent = 1;
    }
}
