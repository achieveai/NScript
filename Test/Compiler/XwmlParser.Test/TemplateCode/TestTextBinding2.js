function getter(src) {
  return src.get_propStr1();
};
function TestTextBinding2_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <h2><span></span> > <span></span> > <span></span></h2> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, null, ""), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 1, 1, null, ""), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 2, 2, null, "")];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(3);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1, 0]);
  objStorage[1] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1, 2]);
  objStorage[2] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1, 4]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 3, 0);
};
TestTextBinding2_var = null;
function TestTextBinding2() {
  if (!TestTextBinding2_var)
    TestTextBinding2_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestTextBinding2_factory, "0");
  return TestTextBinding2_var;
};
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = "";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};