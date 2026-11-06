namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;
    using SunlightUnit;
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>Declares a property on a base class of the template model.</summary>
    public class SetterOnlyVmBase : ObservableObject
    {
        [AutoFire] public string BaseDraft { get; set; }
    }

    /// <summary>
    /// Draft and SubDraft are never assigned from C#: their setters are reached only from
    /// value="@..." and from two-way sub-control bindings.
    /// </summary>
    public class SetterOnlyVm : SetterOnlyVmBase
    {
        [AutoFire] public string Draft { get; set; }
        [AutoFire] public string SubDraft { get; set; }
        public ObservableCollection<SetterOnlyRow> Rows { get; private set; }

        public SetterOnlyVm()
        {
            this.Rows = new ObservableCollection<SetterOnlyRow>();
        }
    }

    /// <summary>Row whose Draft is likewise never assigned from C#.</summary>
    public class SetterOnlyRow : ObservableObject
    {
        [AutoFire] public string Draft { get; set; }
        [AutoFire] public string SubDraft { get; set; }
    }

    /// <summary>Two-way sub-control that records every instance, in creation order.</summary>
    public class PrTwoWayProbe : UISkinableElement
    {
        public static readonly List<PrTwoWayProbe> Created = new List<PrTwoWayProbe>();
        private string value;

        public PrTwoWayProbe(Element element) : base(element)
        {
            Created.Add(this);
        }

        public static Skin DefaultSkin => null;

        [DefaultDataBinding(Mode = DataBindingMode.TwoWay)]
        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.FirePropertyChanged("Value");
                }
            }
        }
    }

    /// <summary>PR review fixes (PR 146).</summary>
    [TestFixture]
    public class RazorPrReviewTests
    {
        private static Element Mount(object vm, Skin skin)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = skin;
            control.Activate();
            return element;
        }

        private static void Dispatch(Element target, string type)
        {
            var evt = Window.Instance.Document.CreateEvent("Event");
            evt.InitEvent(type, true, true);
            target.DispatchEvent(evt);
        }

        // C1: the first edit of a value= input writes through a setter only the template uses.
        [Test]
        public static void TestValueWritesThroughTemplateOnlySetter(Assert assert)
        {
            var vm = new SetterOnlyVm();
            vm.Rows.Add(new SetterOnlyRow());
            var root = Mount(vm, RazorSkinTemplatesClass.PrValueSetterOnly);
            var input = (InputElement)root.QuerySelector("[data-test=vs-model]");
            var rowInput = (InputElement)root.QuerySelector("[data-test=vs-row]");
            assert.Equal(input.Value, "", "null Draft shows empty");

            input.Value = "m1";
            Dispatch(input, "input");
            assert.Equal(vm.Draft, "m1", "first edit writes Model.Draft");

            rowInput.Value = "r1";
            Dispatch(rowInput, "change");
            assert.Equal(vm.Rows[0].Draft, "r1", "first edit writes row.Draft");
        }

        private static string SetValue(PrTwoWayProbe probe, string value)
        {
            try
            {
                probe.Value = value;
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // D-1: a two-way sub-control writes back through a setter only the template uses,
        // including one declared on a base class of the model.
        [Test]
        public static void TestSubControlWritesThroughTemplateOnlySetter(Assert assert)
        {
            PrTwoWayProbe.Created.Clear();
            var vm = new SetterOnlyVm();
            vm.Rows.Add(new SetterOnlyRow());
            Mount(vm, RazorSkinTemplatesClass.PrSubControlSetterOnly);
            assert.Equal(PrTwoWayProbe.Created.Count, 3, "three probes");
            if (PrTwoWayProbe.Created.Count != 3) return;

            assert.Equal(SetValue(PrTwoWayProbe.Created[0], "s1"), null, "Model.SubDraft edit does not throw");
            assert.Equal(vm.SubDraft, "s1", "edit writes Model.SubDraft");
            assert.Equal(SetValue(PrTwoWayProbe.Created[1], "b1"), null, "Model.BaseDraft edit does not throw");
            assert.Equal(vm.BaseDraft, "b1", "edit writes base-class Model.BaseDraft");
            assert.Equal(SetValue(PrTwoWayProbe.Created[2], "r1"), null, "row.SubDraft edit does not throw");
            assert.Equal(vm.Rows[0].SubDraft, "r1", "edit writes row.SubDraft");
        }
    }
}
