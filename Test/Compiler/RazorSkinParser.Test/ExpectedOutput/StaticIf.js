var tmplStore = new Array(1);
var StaticIf_var = null;

function StaticIf_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><div>Static content</div></span>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function StaticIf() {
  if (!StaticIf_var)
    StaticIf_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, PlainVM, StaticIf_factory, "0");
  return StaticIf_var;
}
