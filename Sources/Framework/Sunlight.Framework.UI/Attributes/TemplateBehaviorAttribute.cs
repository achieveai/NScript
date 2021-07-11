//-----------------------------------------------------------------------
// <copyright file="TemplateBehaviorAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Attributes
{
    using System;

    /// <summary>
    /// Definition for TemplateBehaviorAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TemplateBehaviorAttribute
    {
        private string behaviorName;
        public TemplateBehaviorAttribute(string behaviorName)
        { this.behaviorName = behaviorName; }

        public string BehaviorName
        { get { return this.behaviorName; } }
    }
}
