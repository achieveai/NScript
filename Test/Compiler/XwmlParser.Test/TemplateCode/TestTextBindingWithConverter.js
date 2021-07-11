function getter(src) {
  return src.get_propInt1();
};
function converter(from) {
  return Sunlight__Framework__UI__Test__ConverterCollection__ZeroToNullOrToString(from);
};
function TestTextBindingWithConverter_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <h2></h2> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropInt1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, converter, "")];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 1, 0);
};
TestTextBindingWithConverter_var = null;
function TestTextBindingWithConverter() {
  if (!TestTextBindingWithConverter_var)
    TestTextBindingWithConverter_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestTextBindingWithConverter_factory, "0");
  return TestTextBindingWithConverter_var;
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