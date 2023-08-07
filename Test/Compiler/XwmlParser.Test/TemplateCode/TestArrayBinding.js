function getter(src) {
  return src.get_array().gl();
}
function gettera(src) {
  return src.get_array();
}
function TestArrayBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div></div> <div></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [SkinBinderInfo_factory([getter], ["Array"], SkinBinderHelper__SetTextContent, 17, 0, 0, null, ""), SkinBinderInfo_factory([gettera], ["Array"], SkinBinderHelper__SetTextContent, 17, 1, 1, null, "")];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(2);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  objStorage[1] = SkinBinderHelper__GetElementFromPath(htmlRoot, [3]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 2, 0);
}
TestArrayBinding_var = null;
function TestArrayBinding() {
  if (!TestArrayBinding_var)
    TestArrayBinding_var = Skin_factory(UISkinableElement, TestViewModelA, TestArrayBinding_factory, "0");
  return TestArrayBinding_var;
}
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = "";
    doc.head.appendChild(style);
  }
  return doc.stateStore;
}