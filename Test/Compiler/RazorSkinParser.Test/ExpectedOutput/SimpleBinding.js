var SimpleBinding_tmplStore = new Array(1);
var SimpleBinding_var = null;

function SimpleBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestModel\n\n<div>\n    <span><span></span></span>\n</div>";
    SimpleBinding_tmplStore[0] = SimpleBinding_tmplStore[0] ? SimpleBinding_tmplStore[0] : [
      Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([function(dc) { return dc.get_name(); }], [], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 1, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [2]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, SimpleBinding_tmplStore[0], null, 0, 0);
}

function SimpleBinding() {
  if (!SimpleBinding_var)
    SimpleBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestModel, SimpleBinding_factory, "0");
  return SimpleBinding_var;
}
