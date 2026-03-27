var ControlBinding_tmplStore = new Array(1);
var ControlBinding_var = null;

function ControlBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestVM\n\n<div>Content</div>";
    ControlBinding_tmplStore[0] = ControlBinding_tmplStore[0] ? ControlBinding_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, ControlBinding_tmplStore[0], null, 0, 0);
}

function ControlBinding() {
  if (!ControlBinding_var)
    ControlBinding_var = Sunlight__Framework__UI__Skin_factory(MyControl, TestVM, ControlBinding_factory, "0");
  return ControlBinding_var;
}
