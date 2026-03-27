var EventLambda_tmplStore = new Array(1);
var EventLambda_var = null;

function EventLambda_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "TestVM\n\n<button>Cancel</button>";
    EventLambda_tmplStore[0] = EventLambda_tmplStore[0] ? EventLambda_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, EventLambda_tmplStore[0], null, 0, 0);
}

function EventLambda() {
  if (!EventLambda_var)
    EventLambda_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, EventLambda_factory, "0");
  return EventLambda_var;
}
