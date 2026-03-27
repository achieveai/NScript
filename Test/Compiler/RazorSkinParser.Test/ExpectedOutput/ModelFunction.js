function FullName(dc) { return dc.get_firstName() + " " + dc.get_lastName(); }

var tmplStore = new Array(1);
var ModelFunction_var = null;

function ModelFunction_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div><span></span></div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [
      SkinBinderInfo_factory([function(dc) { return dc.get_name(); }], ["Name"], SetTextContent, 17, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 1, 0);
}

function ModelFunction() {
  if (!ModelFunction_var)
    ModelFunction_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, ModelFunction_factory, "0");
  return ModelFunction_var;
}
