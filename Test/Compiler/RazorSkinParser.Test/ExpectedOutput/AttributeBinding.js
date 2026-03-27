var AttributeBinding_tmplStore = new Array(1);
var AttributeBinding_var = null;

function AttributeBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div>Hello</div>";
    AttributeBinding_tmplStore[0] = AttributeBinding_tmplStore[0] ? AttributeBinding_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, AttributeBinding_tmplStore[0], null, 0, 0);
}

function AttributeBinding() {
  if (!AttributeBinding_var)
    AttributeBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, AttributeBinding_factory, "0");
  return AttributeBinding_var;
}
