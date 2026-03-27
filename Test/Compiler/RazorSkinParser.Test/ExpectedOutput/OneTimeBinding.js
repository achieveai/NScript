var OneTimeBinding_tmplStore = new Array(1);
var OneTimeBinding_var = null;

function OneTimeBinding_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = "<div><span></span></div>";
    OneTimeBinding_tmplStore[0] = OneTimeBinding_tmplStore[0] ? OneTimeBinding_tmplStore[0] : [
      Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([function(dc) { return dc.get_appVersion(); }], [], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 1, 0, 0, null, "")
    ];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, OneTimeBinding_tmplStore[0], null, 0, 0);
}

function OneTimeBinding() {
  if (!OneTimeBinding_var)
    OneTimeBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight__Framework__UI__UISkinableElement, PlainVM, OneTimeBinding_factory, "0");
  return OneTimeBinding_var;
}
