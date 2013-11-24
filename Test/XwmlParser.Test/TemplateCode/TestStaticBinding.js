function getter(src) {
  return Sunlight__Framework__UI__Test__TestViewModelB__get_StaticProp();
};
function gettera(src) {
  return src.get_propStr1();
};
function TestStaticBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div><span></span></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter, gettera], ["StaticProp", "PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 18, 0, 0, null, "")];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1, 0]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 1, 0);
};
TestStaticBinding_var = null;
function TestStaticBinding() {
  if (!TestStaticBinding_var)
    TestStaticBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestStaticBinding_factory, "0");
  return TestStaticBinding_var;
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