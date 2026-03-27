var ReactiveForeach_tmplStore = new Array(1);
var ReactiveForeach_var = null;

function ReactiveForeach_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><li><span></span></li></span>";
    ReactiveForeach_tmplStore[0] = ReactiveForeach_tmplStore[0] ? ReactiveForeach_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive collection binders
  var _collBinder_0 = new Sunlight__Framework__UI__Helpers__CollectionBinder(htmlRoot,
    (function() { var e = doc.createElement('div'); e.innerHTML = "<li><span></span></li>"; return e; }()),
    function(tmpl, item) { return tmpl.cloneNode(true); });
  _collBinder_0.get_collection = function(dc) { return dc.get_items(); };
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, ReactiveForeach_tmplStore[0], null, 0, 0);
}

function ReactiveForeach() {
  if (!ReactiveForeach_var)
    ReactiveForeach_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, ReactiveForeach_factory, "0");
  return ReactiveForeach_var;
}
