namespace Sunlight.Framework.UI.Test
{
    using System;
    using System.Collections.Generic;
    using Sunlight.Framework.Observables;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// A view model that counts the listeners bindings hold on it: bindings add and remove them
    /// through <see cref="INotifyPropertyChanged"/>, which this class implements again.
    /// </summary>
    public class ListenerProbeVM : ObservableObject, INotifyPropertyChanged
    {
        private string name;

        private ListenerProbeVM child;

        public int Listeners;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name == value) return;
                this.name = value;
                this.FirePropertyChanged("Name");
            }
        }

        public ListenerProbeVM Child
        {
            get { return this.child; }
            set
            {
                if (this.child == value) return;
                this.child = value;
                this.FirePropertyChanged("Child");
            }
        }

        public new void AddPropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.Listeners++;
            base.AddPropertyChangedListener(propertyName, callback);
        }

        public new void RemovePropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.Listeners--;
            base.RemovePropertyChangedListener(propertyName, callback);
        }
    }

    /// <summary>
    /// A skin instance disposed in the same task as its activation: the activation's queued
    /// work runs after the dispose and must not wire the view model again (a live re-skin
    /// that builds and drops an instance at once, or any Skin swap within one task).
    /// </summary>
    [TestFixture]
    public class SkinInstanceDisposeTests
    {
        [TestCaseSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestRazorSkinDisposedBeforeQueuedActivation_DoesNotListen(Assert assert)
        {
            var timer = new TimeoutQueueTimer();
            TaskScheduler.Instance = new TaskScheduler(timer, 10, 10);
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new ListenerProbeVM();
            vm.Name = "first";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorDisposeProbe;
            control.Activate();
            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "The skin renders before its queued activation runs");
            assert.Equal(1, timer.PendingCount, "The activation's work waits in the scheduler");
            assert.Equal(1, vm.Listeners, "The skin listens to Name");

            control.Skin = null;
            assert.Equal(vm.Listeners, 0, "Disposing the skin removes its listener");
            timer.RunAll();
            assert.Equal(vm.Listeners, 0, "The queued activation of a disposed skin adds none back");

            vm.Name = "changed";
            assert.Equal(span.TextContent, "first", "The disposed skin's DOM is not updated");
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestXwmlSkinDisposedBeforeQueuedActivation_DoesNotListen(Assert assert)
        {
            var timer = new TimeoutQueueTimer();
            TaskScheduler.Instance = new TaskScheduler(timer, 10, 10);
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_AttrBinding;
            control.Activate();
            var elem = element.QuerySelector("[test]");
            assert.NotEqual(null, elem, "The skin renders before its queued activation runs");
            assert.Equal(1, timer.PendingCount, "The activation's subscriptions wait in the scheduler");

            control.Skin = null;
            timer.RunAll();
            vm.PropStr1 = "changed";

            assert.Equal(elem.GetAttribute("test1"), null, "The disposed skin's OneWay binding is not live");
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        /// <summary>
        /// A disposed XWML skin removes the listeners of its OneWay bindings: one on the view
        /// model for <c>{Name}</c>, one on each object along <c>{Child.Name}</c>.
        /// </summary>
        [Test]
        public static void TestXwmlSkinDisposed_RemovesItsOneWayListeners(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new ListenerProbeVM();
            var child = new ListenerProbeVM();
            vm.Child = child;
            child.Name = "child";
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateListenerProbe;
            control.Activate();
            assert.Equal(element.QuerySelector("[test]").GetAttribute("test2"), "child", "The skin shows the path's value");
            assert.Equal(vm.Listeners, 2, "The skin listens to Name and Child");
            assert.Equal(child.Listeners, 1, "The skin listens to Child's Name");

            control.Skin = null;
            assert.Equal(vm.Listeners, 0, "A one-step path stops listening");
            assert.Equal(child.Listeners, 0, "A two-step path stops listening at each step");
        }

        /// <summary>Runs everything at once except timeouts, which the scheduler's low-priority work uses.</summary>
        private class TimeoutQueueTimer : IWindowTimer
        {
            private readonly Queue<Action> timeouts = new Queue<Action>();
            private int nextHandle = 1;

            public int PendingCount
            {
                get { return this.timeouts.Count; }
            }

            // -1: no timer is pending once the action has run, so the scheduler queues the next one.
            public int SetImmediate(Action action)
            {
                action();
                return -1;
            }

            public int SetTimeout(Action action, int timoutTime)
            {
                this.timeouts.Enqueue(action);
                return this.nextHandle++;
            }

            public int SetInterval(Action action, int intervalTime)
            {
                throw new NotImplementedException();
            }

            public void ClearTimeout(int timeoutHandle)
            {
            }

            public void ClearInterval(int intervalHandle)
            {
            }

            public int RequestAnimationFrame(Action action)
            {
                action();
                return -1;
            }

            public void QueueMicrotask(Action action)
            {
                action();
            }

            public void RunAll()
            {
                while (this.timeouts.Count > 0)
                {
                    Action action = this.timeouts.Dequeue();
                    action();
                }
            }
        }
    }
}
