namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    /// <summary>
    /// Razor skin template registrations for browser-based tests.
    /// Each [Skin] property points to a .skin.cshtml embedded resource.
    /// The RazorTemplatingPlugin compiles these at build time.
    /// </summary>
    public class RazorSkinTemplatesClass
    {
        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlLiterals.skin.cshtml")]
        public static Skin RazorSubControlLiterals
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlTopLevel.skin.cshtml")]
        public static Skin RazorSubControlTopLevel
        {
            get { return null; }
        }

        [Skin("RazorSubControlTopLevel")]
        public static Skin RazorSubControlTopLevelShort
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDataContext.skin.cshtml")]
        public static Skin RazorSubControlDataContext
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDefaultContext.skin.cshtml")]
        public static Skin RazorSubControlDefaultContext
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlLifecycle.skin.cshtml")]
        public static Skin RazorSubControlLifecycle
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlForeach.skin.cshtml")]
        public static Skin RazorSubControlForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlForeachSources.skin.cshtml")]
        public static Skin RazorSubControlForeachSources
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlForeachDefaultContext.skin.cshtml")]
        public static Skin RazorSubControlForeachDefaultContext
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlConditional.skin.cshtml")]
        public static Skin RazorSubControlConditional
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDelegate.skin.cshtml")]
        public static Skin RazorSubControlDelegate
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDelegateForeach.skin.cshtml")]
        public static Skin RazorSubControlDelegateForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlItemDelegate.skin.cshtml")]
        public static Skin RazorSubControlItemDelegate
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDomEvent.skin.cshtml")]
        public static Skin RazorSubControlDomEvent
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorProbeAlternate.skin.cshtml")]
        public static Skin RazorProbeAlternate
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlComposition.skin.cshtml")]
        public static Skin RazorSubControlComposition
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorBatchNestedHost.skin.cshtml")]
        public static Skin RazorBatchNestedHost
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorBatchNested.skin.cshtml")]
        public static Skin RazorBatchNested
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorGateVoidEvent.skin.cshtml")]
        public static Skin RazorGateVoidEvent
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorGateOnlyHandler.skin.cshtml")]
        public static Skin RazorGateOnlyHandler
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSiblingVoidEvents.skin.cshtml")]
        public static Skin RazorSiblingVoidEvents
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorGatedMultiEvent.skin.cshtml")]
        public static Skin RazorGatedMultiEvent
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlPart.skin.cshtml")]
        public static Skin RazorSubControlPart
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlTwoWay.skin.cshtml")]
        public static Skin RazorSubControlTwoWay
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlDeactivation.skin.cshtml")]
        public static Skin RazorSubControlDeactivation
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubControlIfInForeach.skin.cshtml")]
        public static Skin RazorSubControlIfInForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSimpleText.skin.cshtml")]
        public static Skin RazorSimpleText
        {
            get
            { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorDisposeProbe.skin.cshtml")]
        public static Skin RazorDisposeProbe
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorOneTimeText.skin.cshtml")]
        public static Skin RazorOneTimeText
        {
            get
            { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorMultiBinding.skin.cshtml")]
        public static Skin RazorMultiBinding
        {
            get
            { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.GraphSimpleText.skin.cshtml")]
        public static Skin GraphSimpleText
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.GraphMultiBinding.skin.cshtml")]
        public static Skin GraphMultiBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorClassBinding.skin.cshtml")]
        public static Skin RazorClassBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorStyleBinding.skin.cshtml")]
        public static Skin RazorStyleBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorAttrBinding.skin.cshtml")]
        public static Skin RazorAttrBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorMultiAttr.skin.cshtml")]
        public static Skin RazorMultiAttr
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorComputed.skin.cshtml")]
        public static Skin RazorComputed
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfOnly.skin.cshtml")]
        public static Skin RazorIfOnly
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfElse.skin.cshtml")]
        public static Skin RazorIfElse
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfElseIf.skin.cshtml")]
        public static Skin RazorIfElseIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfBindings.skin.cshtml")]
        public static Skin RazorIfBindings
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorNestedIf.skin.cshtml")]
        public static Skin RazorNestedIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorNestedIfInElement.skin.cshtml")]
        public static Skin RazorNestedIfInElement
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorStaticIf.skin.cshtml")]
        public static Skin RazorStaticIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeach.skin.cshtml")]
        public static Skin RazorForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeachBindings.skin.cshtml")]
        public static Skin RazorForeachBindings
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfInForeach.skin.cshtml")]
        public static Skin RazorIfInForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeachInIf.skin.cshtml")]
        public static Skin RazorForeachInIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorEventClick.skin.cshtml")]
        public static Skin RazorEventClick
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorEventLambda.skin.cshtml")]
        public static Skin RazorEventLambda
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorTodoApp.skin.cshtml")]
        public static Skin RazorTodoApp
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorStyledTemplate.skin.cshtml")]
        public static Skin RazorStyledTemplate
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorMultiStyled.skin.cshtml")]
        public static Skin RazorMultiStyled
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeachMixedMarkers.skin.cshtml")]
        public static Skin RazorForeachMixedMarkers
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB2IfTop.skin.cshtml")]
        public static Skin AwbB2IfTop
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB2IfLoop.skin.cshtml")]
        public static Skin AwbB2IfLoop
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB4EmptyBeforeLoop.skin.cshtml")]
        public static Skin AwbB4EmptyBeforeLoop
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB5Table.skin.cshtml")]
        public static Skin AwbB5Table
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB7TwinControls.skin.cshtml")]
        public static Skin AwbB7TwinControls
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB8Disabled.skin.cshtml")]
        public static Skin AwbB8Disabled
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB9TwoWay.skin.cshtml")]
        public static Skin AwbB9TwoWay
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.PrValueSetterOnly.skin.cshtml")]
        public static Skin PrValueSetterOnly
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.PrSubControlSetterOnly.skin.cshtml")]
        public static Skin PrSubControlSetterOnly
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB4bHrBeforeLoop.skin.cshtml")]
        public static Skin AwbB4bHrBeforeLoop
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbB5bRowEvents.skin.cshtml")]
        public static Skin AwbB5bRowEvents
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF1RawText.skin.cshtml")]
        public static Skin AwbF1RawText
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF2HandlerOwned.skin.cshtml")]
        public static Skin AwbF2HandlerOwned
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF2RowAndGate.skin.cshtml")]
        public static Skin AwbF2RowAndGate
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF3Select.skin.cshtml")]
        public static Skin AwbF3Select
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF9Zero.skin.cshtml")]
        public static Skin AwbF9Zero
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF10EventAfterIf.skin.cshtml")]
        public static Skin AwbF10EventAfterIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR143Grid.skin.cshtml")]
        public static Skin AwbR143Grid
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR144NullAttr.skin.cshtml")]
        public static Skin AwbR144NullAttr
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR145Lists.skin.cshtml")]
        public static Skin AwbR145Lists
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR3GateLoop.skin.cshtml")]
        public static Skin AwbR3GateLoop
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR3AddThrows.skin.cshtml")]
        public static Skin AwbR3AddThrows
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbR3SourceWalk.skin.cshtml")]
        public static Skin AwbR3SourceWalk
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotGatedFrame.skin.cshtml")]
        public static Skin SlotGatedFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotElseFrame.skin.cshtml")]
        public static Skin SlotElseFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotNoOutletFrame.skin.cshtml")]
        public static Skin SlotNoOutletFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotForwardFrame.skin.cshtml")]
        public static Skin SlotForwardFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotHost.skin.cshtml")]
        public static Skin SlotHost
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotHostRows.skin.cshtml")]
        public static Skin SlotHostRows
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotHostNamed.skin.cshtml")]
        public static Skin SlotHostNamed
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotHostMissingSlot.skin.cshtml")]
        public static Skin SlotHostMissingSlot
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotNestedFrame.skin.cshtml")]
        public static Skin SlotNestedFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotGatedForwardFrame.skin.cshtml")]
        public static Skin SlotGatedForwardFrame
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.AwbF2NestedGate.skin.cshtml")]
        public static Skin AwbF2NestedGate
        {
            get { return null; }
        }
    }
}
