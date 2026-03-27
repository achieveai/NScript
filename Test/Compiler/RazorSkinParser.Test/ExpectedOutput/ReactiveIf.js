var ReactiveIf_tmplStore = new Array(1);
var ReactiveIf_var = null;

function ReactiveIf_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><div>Active</div></span><span><div>Inactive</div></span>";
    ReactiveIf_tmplStore[0] = ReactiveIf_tmplStore[0] ? ReactiveIf_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive conditional binders
  new Sunlight__Framework__UI__Helpers__ConditionalBinder(function(dc) { return dc.get_isActive(); }, ["IsActive"],
    htmlRoot,
    (function() { var e = doc.createElement('div'); e.innerHTML = "<div>Active</div>"; return e; }()),
    (function() { var e = doc.createElement('div'); e.innerHTML = "<div>Inactive</div>"; return e; }()));
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, ReactiveIf_tmplStore[0], null, 0, 0);
}

function ReactiveIf() {
  if (!ReactiveIf_var)
    ReactiveIf_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, ReactiveIf_factory, "0");
  return ReactiveIf_var;
}
