var tmplStore = new Array(1);
var SimpleBinding_var = null;

function SimpleBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestModel\n\n<div>\n    <span><span></span></span>\n</div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [
      SkinBinderInfo_factory([function(dc) { return dc.get_name(); }], [], SetTextContent, 1, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = GetElementFromPath(htmlRoot, [2]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function SimpleBinding() {
  if (!SimpleBinding_var)
    SimpleBinding_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestModel, SimpleBinding_factory, "0");
  return SimpleBinding_var;
}
