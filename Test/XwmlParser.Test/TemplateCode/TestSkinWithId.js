tmplStore = new Array(2);
function TestSkinWithId_test_factory(skinFactory, doc) {
  var htmlRoot, objStorage;
  if (!tmplStore[0]) {
    tmplStore[0] = document.createElement("div");
    tmplStore[0].innerHTML = "<div> <div>Te<span>st</span></div> </div>";
    tmplStore[1] = [];
  }
  htmlRoot = tmplStore[0].cloneNode(true);
  objStorage = [];
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1]);
};
function TestSkinWithId_test() {
  if (!TestSkinWithId_test_var)
    TestSkinWithId_test_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestSkinWithId_test_factory, "0");
  return TestSkinWithId_test_var;
};