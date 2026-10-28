//-----------------------------------------------------------------------
// <copyright file="KnownTemplateTypesTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// #118: a project that does not reference a DLL the templating plugin needs gets an error
    /// naming the type and the DLL, not "Value cannot be null (declaringType)".
    /// </summary>
    [TestClass]
    public class KnownTemplateTypesTests
    {
        [TestMethod]
        public void MissingUiReference_ErrorNamesTheTypeAndTheDll()
        {
            var context = Helper.CreateClrContextWithout("Sunlight.Framework.UI.dll");

            var ex = Assert.ThrowsException<InvalidOperationException>(
                () => new KnownTemplateTypes(context.KnownReferences));

            StringAssert.Contains(ex.Message, "Sunlight.Framework.UI.UIElement");
            StringAssert.Contains(ex.Message, "Add a reference to Sunlight.Framework.UI");
        }

        [TestMethod]
        public void AllReferences_Resolve()
        {
            var context = Helper.CreateClrContextWithout(null);

            var known = new KnownTemplateTypes(context.KnownReferences);

            Assert.IsNotNull(known.SkinInstanceCtor);
        }
    }
}
