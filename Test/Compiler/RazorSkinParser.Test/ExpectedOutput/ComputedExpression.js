var ComputedExpression_tmplStore = new Array(1);
var ComputedExpression_var = null;

function ComputedExpression_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<span><span></span></span>";
    ComputedExpression_tmplStore[0] = ComputedExpression_tmplStore[0] ? ComputedExpression_tmplStore[0] : [
      Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([function(dc) { return dc.get_price() * dc.get_quantity(); }], ["Price", "Quantity"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [0, 0]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, ComputedExpression_tmplStore[0], null, 1, 0);
}

function ComputedExpression() {
  if (!ComputedExpression_var)
    ComputedExpression_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, ComputedExpression_factory, "0");
  return ComputedExpression_var;
}
