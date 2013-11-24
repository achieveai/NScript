function getter(src) {
  return src.get_propInt1();
};
function setter(tar, val) {
  tar.set_oneWayLooseBinding(val);
};
function gettera(src) {
  return src.get_propStr1();
};
function TestUIElementPropertyAttrBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div>This is a test.</div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropInt1"], setter, 17, 0, 0, null, 10), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factorya([gettera], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute, "testAttr", 113, 0, null, null, 0)];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Test__TestUIElement_factory(Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]));
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [0], objStorage, tmplStore[0], null, 1, 0);
};
TestUIElementPropertyAttrBinding_var = null;
function TestUIElementPropertyAttrBinding() {
  if (!TestUIElementPropertyAttrBinding_var)
    TestUIElementPropertyAttrBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestUIElementPropertyAttrBinding_factory, "0");
  return TestUIElementPropertyAttrBinding_var;
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