var tmplStore = new Array(1);
var EventMethodRef_var = null;

function EventMethodRef_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestVM\n\n<button>Submit</button>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function EventMethodRef() {
  if (!EventMethodRef_var)
    EventMethodRef_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, EventMethodRef_factory, "0");
  return EventMethodRef_var;
}
