var tmplStore = new Array(1);
var AttributeBinding_var = null;

function AttributeBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestVM\n\n<div>Hello</div>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function AttributeBinding() {
  if (!AttributeBinding_var)
    AttributeBinding_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, AttributeBinding_factory, "0");
  return AttributeBinding_var;
}
