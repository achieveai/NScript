function delgateGetter(src) {
  return function(arg0, arg1) {
    return src.domEvent1(arg0, arg1);
  };
};
function TestDomEventInFormBinding1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div class=\"problemManagerHeading\"> <h2>A1</h2> <h3>H3</h3> <h4>H4</h4> </div> <div class=\"uploadArea\"> <div>Batch Upload</div> <div style=\"height: 100px; border: 1px solid black;\"> Work in Progress <form enctype=\"multipart/form-data\" method=\"post\"> <label for=\"ftu\">Select file to load.</label> <input name=\"ftu\" type=\"file\" accept=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document\"> </form> </div> </div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([delgateGetter], null, "change", 65, 0, null, null, 0)];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [3, 3, 1, 3]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 1);
};
TestDomEventInFormBinding1_var = null;
function TestDomEventInFormBinding1() {
  if (!TestDomEventInFormBinding1_var)
    TestDomEventInFormBinding1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestDomEventInFormBinding1_factory, "0");
  return TestDomEventInFormBinding1_var;
};
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = ".uploadArea{}.problemManagerHeading{}";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};