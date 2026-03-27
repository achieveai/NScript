var tmplStore = new Array(1);
var TextBinding_var = null;

function TextBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestVM\n\n<div><span></span></div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [
      SkinBinderInfo_factory([function(dc) { return dc.get_name(); }], ["Name"], SetTextContent, 17, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = GetElementFromPath(htmlRoot, [2]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 1, 0);
}

function TextBinding() {
  if (!TextBinding_var)
    TextBinding_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, TextBinding_factory, "0");
  return TextBinding_var;
}
