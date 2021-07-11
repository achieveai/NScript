function getter(src) {
  return src.get_array().V_get_Length();
};
function gettera(src) {
  return src.get_array();
};
function TestArrayBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div></div> <div></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["Array"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, null, ""), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([gettera], ["Array"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 1, 1, null, "")];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(2);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  objStorage[1] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [3]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 2, 0);
};
TestArrayBinding_var = null;
function TestArrayBinding() {
  if (!TestArrayBinding_var)
    TestArrayBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestArrayBinding_factory, "0");
  return TestArrayBinding_var;
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