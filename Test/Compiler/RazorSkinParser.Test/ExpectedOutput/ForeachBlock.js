var tmplStore = new Array(1);
var ForeachBlock_var = null;

function ForeachBlock_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><li><span></span></li></span>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive collection binders
  CollectionBinder_setup(htmlRoot, function(dc) { return dc.get_items(); },
    "<li><span></span></li>");
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function ForeachBlock() {
  if (!ForeachBlock_var)
    ForeachBlock_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, ForeachBlock_factory, "0");
  return ForeachBlock_var;
}
