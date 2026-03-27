var tmplStore = new Array(1);
var ReactiveForeach_var = null;

function ReactiveForeach_factory(skinFactory, doc) {
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

function ReactiveForeach() {
  if (!ReactiveForeach_var)
    ReactiveForeach_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, ReactiveForeach_factory, "0");
  return ReactiveForeach_var;
}
