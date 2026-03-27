var tmplStore = new Array(1);
var ConditionalBlock_var = null;

function ConditionalBlock_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><div>Active</div></span><span><div>Inactive</div></span>";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(0);
  // Reactive conditional binders
  ConditionalBinder_setup(htmlRoot, function(dc) { return dc.get_isActive(); }, ["IsActive"],
    "<div>Active</div>",
    "<div>Inactive</div>");
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}

function ConditionalBlock() {
  if (!ConditionalBlock_var)
    ConditionalBlock_var = Skin_factory(Sunlight.Framework.UI.UISkinableElement, TestVM, ConditionalBlock_factory, "0");
  return ConditionalBlock_var;
}
