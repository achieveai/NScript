var NestedControlFlow_tmplStore = new Array(1);
var NestedControlFlow_var = null;

function NestedControlFlow_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><span><li class=\"done\"><span></span></li></span></span>";
    NestedControlFlow_tmplStore[0] = NestedControlFlow_tmplStore[0] ? NestedControlFlow_tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive collection binders
  var _collBinder_0 = new Sunlight__Framework__UI__Helpers__CollectionBinder(htmlRoot,
    (function() { var e = doc.createElement('div'); e.innerHTML = "<span><li class=\"done\"><span></span></li></span>"; return e; }()),
    function(tmpl, item) { return tmpl.cloneNode(true); });
  _collBinder_0.get_collection = function(dc) { return dc.get_items(); };
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, NestedControlFlow_tmplStore[0], null, 0, 0);
}

function NestedControlFlow() {
  if (!NestedControlFlow_var)
    NestedControlFlow_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, NestedControlFlow_factory, "0");
  return NestedControlFlow_var;
}
