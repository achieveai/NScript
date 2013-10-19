function getter(src) {
  src.get_propStr1();
};
tmplStore = new Array(2);
function TestTextBinding1_factory(skinFactory, doc) {
  var objStorage, htmlRoot;
  if (!tmplStore[0]) {
    tmplStore[0] = document.createElement("div");
    tmplStore[0].innerHTML = "<div> <div><span></span></div> </div>";
    tmplStore[1] = [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, true, 0, null, "")];
  }
  htmlRoot = tmplStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1, 0]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1]);
};
function TestTextBinding1() {
  if (!TestTextBinding1_var)
    TestTextBinding1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestTextBinding1_factory, "0");
  return TestTextBinding1_var;
};