var tmplStore = new Array(1);
var NestedControlFlow_var = null;

function NestedControlFlow_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><span><li class=\"done\"><span></span></li></span></span>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive collection binders
  CollectionBinder_setup(htmlRoot, function(dc) { return dc.get_items(); },
    "<span><li class=\"done\"><span></span></li></span>");
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function NestedControlFlow() {
  if (!NestedControlFlow_var)
    NestedControlFlow_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, NestedControlFlow_factory, "0");
  return NestedControlFlow_var;
}
