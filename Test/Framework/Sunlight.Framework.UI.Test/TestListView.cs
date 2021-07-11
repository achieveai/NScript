//-----------------------------------------------------------------------
// <copyright file="TestListView.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using System.Collections.Generic;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for TestListView
    /// </summary>
    [TestClass]
    public class TestListView
    {
        private static HeaderInjectableTransformer<TestViewModelB, TestViewModelC> _headerTransformer;
        private static NativeArray<Element> _elemSeletor;

        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(
                new TestWindowTimer(),
                10,
                10);
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestChildSkin(Assert assert)
        {
            Document document = Window.Instance.Document;
            ListView listView = new ListView(document.CreateElement("div"));
            listView.ItemSkin = NScriptsTemplatesClass.TestTemplateVMB1;

            TestViewModelB[] vmAs = new TestViewModelB[]
            {
                new TestViewModelB() { PropStr1 = "StrTest"},
                new TestViewModelB() { PropStr1 = "TestStr"},
                new TestViewModelB() { PropStr1 = "TestTT1"}
            };

            listView.FixedList = vmAs;
            var aa = listView.ObservableList;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            var elems = listView.Element.QuerySelectorAll("[test]");
            assert.Equal(3, elems.Length, "number of children should be 3");

            for (int i = 0; i < 3; i++)
            {
                assert.Equal(
                    vmAs[i].PropStr1,
                    elems[i].InnerText,
                    "Inner text for element should match property it's bound to.");
            }
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestHeaderSkin(Assert assert)
        {
            Document document = Window.Instance.Document;
            ListView listView = new ListView(document.CreateElement("div"));
            listView.ItemSkin = NScriptsTemplatesClass.TestTemplateVMB1_int;
            listView.HeaderSkin = NScriptsTemplatesClass.TestTemplateVMC1;

            _headerTransformer = new HeaderInjectableTransformer<TestViewModelB, TestViewModelC>(GenerateHeader);
            //Direct Set
            var coll = new ObservableCollection<TestViewModelB>();
            coll.Add(new TestViewModelB { PropInt2 = 4});
            coll.Add(new TestViewModelB { PropInt2 = 6 });
            coll.Add(new TestViewModelB { PropInt2 = 11 });

            _headerTransformer.InputCollection = coll;
            listView.ObservableList = _headerTransformer.TransformedCollection;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            _elemSeletor = listView.Element.QuerySelectorAll("[test]");
            TestCase(assert);

            //Replace
            coll.Clear();
            coll.Add(new TestViewModelB { PropInt2 = 4 });
            coll.Add(new TestViewModelB { PropInt2 = 5 });
            coll.Add(new TestViewModelB { PropInt2 = 11 });

            _headerTransformer.InputCollection = coll;
            listView.ObservableList = _headerTransformer.TransformedCollection;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            _elemSeletor = listView.Element.QuerySelectorAll("[test]");
            TestCase(assert);

            //Remove
            coll.Clear();
            coll.Add(new TestViewModelB { PropInt2 = 4 });
            coll.Add(new TestViewModelB { PropInt2 = 5 });

            _headerTransformer.InputCollection = coll;
            listView.ObservableList = _headerTransformer.TransformedCollection;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            _elemSeletor = listView.Element.QuerySelectorAll("[test]");
            TestCase(assert);

            //Insert
            coll.Clear();
            coll.Add(new TestViewModelB { PropInt2 = 4 });
            coll.Add(new TestViewModelB { PropInt2 = 6 });
            coll.Add(new TestViewModelB { PropInt2 = 11 });

            _headerTransformer.InputCollection = coll;
            listView.ObservableList = _headerTransformer.TransformedCollection;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            _elemSeletor = listView.Element.QuerySelectorAll("[test]");
            TestCase(assert);

        }

        private static void TestCase(Assert assert)
        {
            var count = _headerTransformer.TransformedCollection.Count;
            assert.Equal(count, _elemSeletor.Length, "number of children should be "+ count);

            for (int i = 0; i < count; i++)
            {
                var element = _headerTransformer.TransformedCollection[i];
                if (element.Header != null)
                {
                    assert.Equal(
                        element.Header.PropStr3,
                        _elemSeletor[i].InnerText,
                        "Inner text for element should match property it's bound to.");
                }
                else
                {
                    assert.Equal(
                        element.Item.PropInt2.ToString(),
                        _elemSeletor[i].InnerText,
                        "Inner text for element should match property it's bound to.");
                }
            }
        }

        public static TestViewModelC GenerateHeader(TestViewModelB first, TestViewModelB second)
        {
            int secondHead = second.PropInt2 / 10;
            string headerText = "Header : ";
            if (first == null)
            {
                return new TestViewModelC { PropStr3 = headerText+secondHead };
            }

            int firstHead = first.PropInt2 / 10;
            if (firstHead == secondHead)
            {
                return null;
            }
            else
            {
                return new TestViewModelC { PropStr3 = headerText + secondHead };
            }
        }

    }
}

