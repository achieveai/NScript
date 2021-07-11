function getter(src) {
  return src.get_isSelected();
};
function TestTemplateBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div>Te<span>st</span></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass, "selected", 83, 0, null, false, 0)];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
};
TestTemplateBinding_var = null;
function TestTemplateBinding() {
  if (!TestTemplateBinding_var)
    TestTemplateBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_ListViewItem, Sunlight_Framework_UI_Test_TestViewModelA, TestTemplateBinding_factory, "0");
  return TestTemplateBinding_var;
};
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = ".selected{}";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};
