function delgateGetter(src) {
  return function(arg0, arg1) {
    return src.domEvent1(arg0, arg1);
  };
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
function TestDomEventBinding1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div> <button>Test Click</button> </div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([delgateGetter], null, "click", 65, 0, null, null, 0)];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 1);
};
function TestDomEventBinding1() {
  if (!TestDomEventBinding1_var)
    TestDomEventBinding1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestDomEventBinding1_factory, "0");
  return TestDomEventBinding1_var;
};