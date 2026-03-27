var tmplStore = new Array(1);
var OneTimeBinding_var = null;

function OneTimeBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "PlainVM\n\n<div><span></span></div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [
      SkinBinderInfo_factory([function(dc) { return dc.get_appVersion(); }], [], SetTextContent, 1, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = GetElementFromPath(htmlRoot, [2]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function OneTimeBinding() {
  if (!OneTimeBinding_var)
    OneTimeBinding_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, PlainVM, OneTimeBinding_factory, "0");
  return OneTimeBinding_var;
}
