function TestSkinWithUIElementDomId_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div test=\"value\">Test</div> <div>Test</div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(3);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  objStorage[1] = Sunlight__Framework__UI__Test__TestUIElementWithAttr_factory(Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]));
  objStorage[2] = Sunlight__Framework__UI__Test__TestUIElement_factory(Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [3]));
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [1, 2], objStorage, tmplStore[0], {
    "DomPart": 0,
    "Part1": 2
  }, 0, 0);
};
TestSkinWithUIElementDomId_var = null;
function TestSkinWithUIElementDomId() {
  if (!TestSkinWithUIElementDomId_var)
    TestSkinWithUIElementDomId_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_Test_TestSkinableWithDomElementPart, Sunlight_Framework_UI_Test_TestViewModelA, TestSkinWithUIElementDomId_factory, "0");
  return TestSkinWithUIElementDomId_var;
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