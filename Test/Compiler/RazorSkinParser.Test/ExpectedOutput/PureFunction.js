function Fmt(x) { return x.get_toString()(); }

var PureFunction_tmplStore = new Array(1);
var PureFunction_var = null;

function PureFunction_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div><span></span></div>";
    PureFunction_tmplStore[0] = PureFunction_tmplStore[0] ? PureFunction_tmplStore[0] : [
      Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([function(dc) { return dc.get_name(); }], ["Name"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [0, 0]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, PureFunction_tmplStore[0], null, 1, 0);
}

function PureFunction() {
  if (!PureFunction_var)
    PureFunction_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, TestVM, PureFunction_factory, "0");
  return PureFunction_var;
}
