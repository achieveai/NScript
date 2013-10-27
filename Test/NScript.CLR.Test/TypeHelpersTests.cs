//-----------------------------------------------------------------------
// <copyright file="TypeHelpersTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.Test;
    using NUnit.Framework;
    using Mono.Cecil;
    using System.Linq;


    /// <summary>
    /// Definition for TypeHelpersTests
    /// </summary>
    [TestFixture]
    public class TypeHelpersTests
    {
        TypeDefinition action1TypeDef,
                        action2TypeDef,
                        func2TypeDef,
                        func3TypeDef;
        private TypeDefinition arrayListTypeDef;
        private TypeDefinition list1TypeDef;
        private TypeDefinition testInheritence;
        private TypeDefinition baseClass;
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
            this.action1TypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "Mscorlib",
                    "System.Action`1")).Resolve();
            this.action2TypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "Mscorlib",
                    "System.Action`2")).Resolve();

            this.func2TypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "Mscorlib",
                    "System.Func`2")).Resolve();

            this.func3TypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "Mscorlib",
                    "System.Func`3")).Resolve();

            arrayListTypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "mscorlib",
                    "System.Collections.ArrayList"));

            list1TypeDef = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "mscorlib",
                    "System.Collections.Generic.List`1"));

            baseClass = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "RealScript",
                    "RealScript.BaseClass"));

            testInheritence = TestAssemblyLoader.Context.GetTypeDefinition(
                Tuple.Create(
                    "RealScript",
                    "RealScript.TestInheritence"));

        }

        [Test]
        public void ImplementsDelegateTestSimple()
        {
            var containsMethod = arrayListTypeDef.Methods.First(m => m.Name == "Contains");
            var indexOfMethod1 = arrayListTypeDef.Methods.First(m => m.Name == "IndexOf" && m.Parameters.Count == 1);
            var everyMethod = arrayListTypeDef.Methods.First(m => m.Name == "Every");

            var func2ObjBool = new GenericInstanceType(this.func2TypeDef);
            func2ObjBool.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Object);
            func2ObjBool.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Boolean);

            Assert.AreEqual(
                0,
                func2ObjBool.ImplementsDelegate(containsMethod),
                "ArrayList.Contains implements Func<object, bool>");
            Assert.AreEqual(
                (int?)null,
                func2ObjBool.ImplementsDelegate(indexOfMethod1), 
                "ArrayList.IndexOf does not implement Func<object, bool>");
            Assert.AreEqual(
                (int?)null,
                func2ObjBool.ImplementsDelegate(everyMethod),
                "ArrayList.Every implements Func<object, bool>");
        }

        [Test]
        public void ImplementsDelegateTestReturnOverload()
        {
            var containsMethod = arrayListTypeDef.Methods.First(m => m.Name == "Contains");
            var indexOfMethod1 = arrayListTypeDef.Methods.First(m => m.Name == "IndexOf" && m.Parameters.Count == 1);
            var everyMethod = arrayListTypeDef.Methods.First(m => m.Name == "Every");

            var func2ObjObj = new GenericInstanceType(this.func2TypeDef);
            func2ObjObj.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Object);
            func2ObjObj.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Object);

            Assert.AreEqual(
                1,
                func2ObjObj.ImplementsDelegate(containsMethod),
                "ArrayList.Contains implements Func<object, Object>");
            Assert.AreEqual(
                1,
                func2ObjObj.ImplementsDelegate(indexOfMethod1),
                "ArrayList.IndexOf does not implement Func<object, Object>");
            Assert.AreEqual(
                (int?)null,
                func2ObjObj.ImplementsDelegate(everyMethod),
                "ArrayList.Every implements Func<object, Object>");
        }

        [Test]
        public void ImplementsDelegateTestMethodIgnore()
        {
            var indexOfMethod1 = arrayListTypeDef.Methods.First(
                m => m.Name == "IndexOf" && m.Parameters.Count == 1);
            var indexOfMethod2 = arrayListTypeDef.Methods.First(
                m => m.Name == "IndexOf" && m.Parameters.Count == 2);

            var func3ObjIntInt = new GenericInstanceType(this.func3TypeDef);
            func3ObjIntInt.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Object);
            func3ObjIntInt.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Int32);
            func3ObjIntInt.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Int32);

            Assert.AreEqual(
                0,
                func3ObjIntInt.ImplementsDelegate(indexOfMethod2),
                "ArrayList.IndexOf(obj, int) implements Func<object, int, int>");
            Assert.AreEqual(
                (int?)null,
                func3ObjIntInt.ImplementsDelegate(indexOfMethod1),
                "ArrayList.IndexOf(obj) does not implement Func<object, int, int>");
            Assert.AreEqual(
                1000,
                func3ObjIntInt.ImplementsDelegate(indexOfMethod1, true),
                "ArrayList.IndexOf(obj) does implement Func<object, int, int> with missingParamsOk");
        }

        [Test]
        public void ImplementsDelegateTestGenericType()
        {
            var list1BaseClass = new GenericInstanceType(list1TypeDef);
            list1BaseClass.GenericArguments.Add(baseClass);

            var action1BaseClass = new GenericInstanceType(action1TypeDef);
            action1BaseClass.GenericArguments.Add(baseClass);

            var action1Inherited = new GenericInstanceType(action1TypeDef);
            action1Inherited.GenericArguments.Add(testInheritence);

            var action1Obj = new GenericInstanceType(action1TypeDef);
            action1Obj.GenericArguments.Add(TestAssemblyLoader.Context.KnownReferences.Object);

            var addMethod = list1TypeDef.Methods.First(m => m.Name == "Add").FixGenericArguments(list1BaseClass);

            Assert.AreEqual(0, action1BaseClass.ImplementsDelegate(addMethod), "List<BaseClass>.Add implements Action<BaseClass>");
            Assert.AreEqual((int?)null, action1Obj.ImplementsDelegate(addMethod), "List<BaseClass>.Add does not implements Action<Object>");
            Assert.AreEqual(10, action1Inherited.ImplementsDelegate(addMethod), "List<BaseClass>.Add does implements Action<TestInheritance>");
        }
    }
}
