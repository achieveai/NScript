//-----------------------------------------------------------------------
// <copyright file="MutableEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for MutableEvent
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    [ScriptName("MutationEvent")]
    public class MutableEvent
    {
        private MutableEvent() { }

        /// <summary>
        /// Inits the custom event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="detail">The detail.</param>
        public extern void InitCustomEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            string detail);

        /// <summary>
        /// Inits the event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        public extern void InitEvent(
            string eventType,
            bool canBubble,
            bool canCancel);

        /// <summary>
        /// Inits the focus event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        public extern void InitFocusEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail);

        /// <summary>
        /// Inits the keyboard event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="key">The key.</param>
        /// <param name="location">The location.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="repeat">The repeat.</param>
        /// <param name="locale">The locale.</param>
        public extern void InitKeyboardEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail,
            string key,
            string location,
            string modifiers,
            int repeat,
            string locale);

        /// <summary>
        /// Inits the mouse event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="clientX">The client X.</param>
        /// <param name="clientY">The client Y.</param>
        /// <param name="ctrlKey">if set to <c>true</c> [CTRL key].</param>
        /// <param name="altKey">if set to <c>true</c> [alt key].</param>
        /// <param name="shiftKey">if set to <c>true</c> [shift key].</param>
        /// <param name="metaKey">if set to <c>true</c> [meta key].</param>
        /// <param name="button">The button.</param>
        /// <param name="relatedTarget">The related target.</param>
        public extern void InitMouseEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail,
            int screenX,
            int screenY,
            int clientX,
            int clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            string button,
            Element relatedTarget);

        /// <summary>
        /// Inits the mouse wheel event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="clientX">The client X.</param>
        /// <param name="clientY">The client Y.</param>
        /// <param name="ctrlKey">if set to <c>true</c> [CTRL key].</param>
        /// <param name="altKey">if set to <c>true</c> [alt key].</param>
        /// <param name="shiftKey">if set to <c>true</c> [shift key].</param>
        /// <param name="metaKey">if set to <c>true</c> [meta key].</param>
        /// <param name="button">The button.</param>
        /// <param name="relatedTarget">The related target.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="wheelDelta">The wheel delta.</param>
        public extern void InitMouseWheelEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail,
            int screenX,
            int screenY,
            int clientX,
            int clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            string button,
            Element relatedTarget,
            string modifiers,
            int wheelDelta);

        /// <summary>
        /// Inits the UI event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        public extern void InitUIEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail);

        /// <summary>
        /// Inits the wheel event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="canBubble">if set to <c>true</c> [can bubble].</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <param name="view">The view.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="clientX">The client X.</param>
        /// <param name="clientY">The client Y.</param>
        /// <param name="ctrlKey">if set to <c>true</c> [CTRL key].</param>
        /// <param name="altKey">if set to <c>true</c> [alt key].</param>
        /// <param name="shiftKey">if set to <c>true</c> [shift key].</param>
        /// <param name="metaKey">if set to <c>true</c> [meta key].</param>
        /// <param name="button">The button.</param>
        /// <param name="relatedTarget">The related target.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        /// <param name="deltaZ">The delta Z.</param>
        /// <param name="deltaMode">The delta mode.</param>
        public extern void InitWheelEvent(
            string eventType,
            bool canBubble,
            bool canCancel,
            Window view,
            string detail,
            int screenX,
            int screenY,
            int clientX,
            int clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            string button,
            Element relatedTarget,
            string modifiers,
            int deltaX,
            int deltaY,
            int deltaZ,
            int deltaMode);
    }
}