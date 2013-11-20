//-----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Document.
    /// </summary>
    [IgnoreNamespace]
    public sealed class Document
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private extern Document();

        /// <summary>
        /// Gets the active element.
        /// </summary>
        /// <value>
        /// The active element.
        /// </value>
        public extern Element ActiveElement
        { get; }

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public extern Element Body
        { get; }

        /// <summary>
        /// Gets or sets the cookie.
        /// </summary>
        /// <value>
        /// The cookie.
        /// </value>
        public extern string Cookie
        { get; set; }

        /// <summary>
        /// Gets or sets the design mode.
        /// </summary>
        /// <value>
        /// The design mode.
        /// </value>
        public extern string DesignMode
        { get; set; }

        /// <summary>
        /// Gets the doctype.
        /// </summary>
        /// <value>
        /// The doctype.
        /// </value>
        public extern string Doctype
        { get; }

        /// <summary>
        /// Gets the document element.
        /// </summary>
        /// <value>
        /// The document element.
        /// </value>
        public extern Element DocumentElement
        { get; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public extern string Domain
        { get; set; }

        /// <summary>
        /// Gets the parent window.
        /// </summary>
        /// <value>
        /// The parent window.
        /// </value>
        public extern Window ParentWindow
        { get; }

        /// <summary>
        /// Gets the referrer.
        /// </summary>
        /// <value>
        /// The referrer.
        /// </value>
        public extern string Referrer
        { get; }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <value>
        /// The selection.
        /// </value>
        public extern Selection Selection
        { get; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public extern string Title
        { get; set; }

        /// <summary>
        /// Gets URL of the document.
        /// </summary>
        /// <value>
        /// The url.
        /// </value>
        public extern string URL
        { get; }

        /// <summary>
        /// Closes this object.
        /// </summary>
        public extern void Close();

        /// <summary>
        /// Creates an attribute.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The new attribute.
        /// </returns>
        public extern NodeAttribute CreateAttribute(string name);

        /// <summary>
        /// Creates the document fragment.
        /// </summary>
        /// <returns>
        /// The new document fragment.
        /// </returns>
        public extern DocumentFragment CreateDocumentFragment();

        /// <summary>
        /// Creates an element.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The new element.
        /// </returns>
        public extern Element CreateElement(string tagName);

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="eventType"> Type of the event. </param>
        /// <returns>
        /// The new event.
        /// </returns>
        public extern MutableEvent CreateEvent(string eventType);

        /// <summary>
        /// Creates a text node.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The new text node.
        /// </returns>
        public extern Element CreateTextNode(string tagName);

        /// <summary>
        /// Dispatch event.
        /// </summary>
        /// <param name="eventObject"> The event object. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool DispatchEvent(MutableEvent eventObject);

        /// <summary>
        /// Element from point.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element ElementFromPoint(int x, int y);

        /// <summary>
        /// Executes the command operation.
        /// </summary>
        /// <param name="command">              The command. </param>
        /// <param name="displayUserInterface"> true to display user interface. </param>
        /// <param name="value">                The value. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool ExecCommand(string command, bool displayUserInterface, object value);

        /// <summary>
        /// Focus this object.
        /// </summary>
        public extern void Focus();

        /// <summary>
        /// Gets an element by identifier.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns>
        /// The element by identifier.
        /// </returns>
        public extern Element GetElementById(string id);

        /// <summary>
        /// Query if this object has focus.
        /// </summary>
        /// <returns>
        /// true if focus, false if not.
        /// </returns>
        public extern bool HasFocus();

        /// <summary>
        /// Opens this object.
        /// </summary>
        public extern void Open();

        /// <summary>
        /// Queries a command enabled.
        /// </summary>
        /// <param name="command"> The command. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool QueryCommandEnabled(string command);

        /// <summary>
        /// Queries a command indeterm.
        /// </summary>
        /// <param name="command"> The command. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool QueryCommandIndeterm(string command);

        /// <summary>
        /// Queries a command state.
        /// </summary>
        /// <param name="command"> The command. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool QueryCommandState(string command);

        /// <summary>
        /// Queries a command supported.
        /// </summary>
        /// <param name="command"> The command. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool QueryCommandSupported(string command);

        /// <summary>
        /// Queries a command value.
        /// </summary>
        /// <param name="command"> The command. </param>
        /// <returns>
        /// The command value.
        /// </returns>
        public extern object QueryCommandValue(string command);

        /// <summary>
        /// Queries a selector.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector.
        /// </returns>
        public extern Element QuerySelector(string selector);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="listener">  The listener to be invoked in response to the event. </param>
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Attaches the event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="handler">   The handler. </param>
        public extern void AttachEvent(string eventName, Action handler);

        /// <summary>
        /// Detaches the event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="handler">   The handler. </param>
        public extern void DetachEvent(string eventName, Action handler);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="listener">  The listener to be invoked in response to the event. </param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Writes.
        /// </summary>
        /// <param name="text"> The string to write. </param>
        public extern void Write(string text);

        /// <summary>
        /// Writelns.
        /// </summary>
        /// <param name="text"> The text. </param>
        public extern void Writeln(string text);

        /// <summary>
        /// Gets the elements by class name internal.
        /// </summary>
        /// <param name="className"> Name of the class. </param>
        /// <returns>
        /// The elements by class name internal.
        /// </returns>
        [ScriptName("getElementsByClassName")]
        internal extern NativeArray<Element> GetElementsByClassName(string className);

        /// <summary>
        /// Gets the elements by name internal.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The elements by name internal.
        /// </returns>
        [ScriptName("getElementsByName")]
        public extern NativeArray<Element> GetElementsByName(string name);

        /// <summary>
        /// Gets the elements by tag name internal.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name internal.
        /// </returns>
        [ScriptName("getElementsByTagName")]
        public extern NativeArray<Element> GetElementsByTagName(string tagName);

        /// <summary>
        /// Queries a selector all internal.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector all internal.
        /// </returns>
        [ScriptName("querySelectorAll")]
        internal extern NativeArray<Element> QuerySelectorAll(string selector);
    }
}