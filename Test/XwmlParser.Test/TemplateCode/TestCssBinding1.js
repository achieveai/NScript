function getter(src) {
  src.get_propBool1();
};
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = ".test{font-weight:bold;}";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};
function TestCssBinding1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div> <div></div> </div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass, "test", true, 0, null, false)];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1]);
};
function TestCssBinding1() {
  if (!TestCssBinding1_var)
    TestCssBinding1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestCssBinding1_factory, "0");
  return TestCssBinding1_var;
};