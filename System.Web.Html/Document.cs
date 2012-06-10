//-----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    [Imported]
    [IgnoreNamespace]
    public sealed class Document
    {
        private Document() { }

        [IntrinsicField]
        public readonly Element ActiveElement;

        [IntrinsicField]
        public readonly Element Body;

        [IntrinsicField]
        public string Cookie;

        [IntrinsicField]
        public string DesignMode;

        [IntrinsicField]
        public readonly string Doctype;

        [IntrinsicField]
        public readonly Element DocumentElement;

        [IntrinsicField]
        public string Domain;

        [IntrinsicField]
        public readonly Window ParentWindow;

        [IntrinsicField]
        public readonly string Referrer;

        [IntrinsicField]
        public readonly Selection Selection;

        [IntrinsicField]
        public string Title;

        [IntrinsicField]
        public readonly string URL;

        public extern void Close();

        public extern NodeAttribute CreateAttribute(string name);

        public extern DocumentFragment CreateDocumentFragment();

        public extern Element CreateElement(string tagName);

        public extern MutableEvent CreateEvent(string eventType);

        public extern Element CreateTextNode(string tagName);

        public extern bool DispatchEvent(MutableEvent eventObject);

        public extern Element ElementFromPoint(int x, int y);

        public extern bool ExecCommand(string command, bool displayUserInterface, object value);

        public extern void Focus();

        public extern Element GetElementById(string id);

        public extern bool HasFocus();

        public extern void Open();

        public extern bool QueryCommandEnabled(string command);

        public extern bool QueryCommandIndeterm(string command);

        public extern bool QueryCommandState(string command);

        public extern bool QueryCommandSupported(string command);

        public extern object QueryCommandValue(string command);

        public extern Element QuerySelector(string selector);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        /// <param name="useCapture">Whether the listener wants to initiate capturing the event.</param>
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Attaches the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler.</param>
        public extern void AttachEvent(string eventName, Action handler);

        /// <summary>
        /// Detaches the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler.</param>
        public extern void DetachEvent(string eventName, Action handler);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        /// <param name="useCapture">Whether the listener wants to initiate capturing the event.</param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        public extern void Write(string text);

        public extern void Writeln(string text);

        public DomList<Node> GetElementsByClassName(string className)
        {
            return new DomList<Node>(this.GetElementsByClassNameInternal(className));
        }

        public DomList<Node> GetElementsByName(string name)
        {
            return new DomList<Node>(this.GetElementsByNameInternal(name));
        }

        public DomList<Node> GetElementsByTagName(string tagName)
        {
            return new DomList<Node>(this.GetElementsByTagNameInternal(tagName));
        }

        public DomList<Node> QuerySelectorAll(string selector)
        {
            return new DomList<Node>(this.QuerySelectorAllInternal(selector));
        }

        [ScriptName("getElementsByClassName")]
        internal extern NativeArray GetElementsByClassNameInternal(string className);

        [ScriptName("getElementsByName")]
        internal extern NativeArray GetElementsByNameInternal(string name);

        [ScriptName("getElementsByTagName")]
        internal extern NativeArray GetElementsByTagNameInternal(string tagName);

        [ScriptName("querySelectorAll")]
        internal extern NativeArray QuerySelectorAllInternal(string selector);
    }
}