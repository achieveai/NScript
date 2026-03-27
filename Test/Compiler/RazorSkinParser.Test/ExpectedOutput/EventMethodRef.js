var EventMethodRef_tmplStore = new Array(1);
var EventMethodRef_var = null;

function EventMethodRef_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<button>Submit</button>";
    EventMethodRef_tmplStore[0] = EventMethodRef_tmplStore[0] ? EventMethodRef_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, EventMethodRef_tmplStore[0], null, 0, 0);
}

function EventMethodRef() {
  if (!EventMethodRef_var)
    EventMethodRef_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, EventMethodRef_factory, "0");
  return EventMethodRef_var;
}
