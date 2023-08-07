function Test$5cTemplates$5cTestRemoveUnusedCss1_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div class=\"testClass\">File1</div> <div class=\"testClassTwo\">File1</div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = [];
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}
Test$5cTemplates$5cTestRemoveUnusedCss1_var = null;
function Test$5cTemplates$5cTestRemoveUnusedCss1() {
  if (!Test$5cTemplates$5cTestRemoveUnusedCss1_var)
    Test$5cTemplates$5cTestRemoveUnusedCss1_var = Skin_factory(UISkinableElement, TestViewModelA, Test$5cTemplates$5cTestRemoveUnusedCss1_factory, "0");
  return Test$5cTemplates$5cTestRemoveUnusedCss1_var;
}
function Test$5cTemplates$5cTestRemoveUnusedCss2_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[1]) {
    domStore[1] = doc.createElement("div");
    domStore[1].innerHTML = " <div class=\"usedInFile2Class\">File2</div> ";
    tmplStore[1] = tmplStore[1] ? tmplStore[1] : [];
  }
  htmlRoot = domStore[1].cloneNode(true);
  objStorage = [];
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1], null, 0, 0);
}
Test$5cTemplates$5cTestRemoveUnusedCss2_var = null;
function Test$5cTemplates$5cTestRemoveUnusedCss2() {
  if (!Test$5cTemplates$5cTestRemoveUnusedCss2_var)
    Test$5cTemplates$5cTestRemoveUnusedCss2_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestRemoveUnusedCss2_factory, "1");
  return Test$5cTemplates$5cTestRemoveUnusedCss2_var;
}
tmplStore = new Array(2);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(2);
    style = doc.createElement("style");
    style.textContent = ".testClass{color:black;\n}\n.testClassTwo{color:white;\n}\n.usedInFile2Class{color:yellow;\n}\n";
    doc.head.appendChild(style);
  }
  return doc.stateStore;
}