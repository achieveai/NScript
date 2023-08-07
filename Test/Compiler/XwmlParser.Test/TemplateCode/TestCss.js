function TestCss_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div class=\"testClass\">Test</div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = [];
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
}
TestCss_var = null;
function TestCss() {
  if (!TestCss_var)
    TestCss_var = Skin_factory(UISkinableElement, TestViewModelA, TestCss_factory, "0");
  return TestCss_var;
}
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = ".testClass[t=ten]>div:hover>#i:nth-child(odd) a:nth-child(2n+1){height:10px;\ncolor:rgba(10,20,30,0.4);\n-webkit-transform:translate3d(10px,40px,10%);\ntransform:translate3d(10px,40px,10%);\n}\n.testClass{-webkit-animation:newAnim 0.5s ease;animation:newAnim 0.5s ease;\n}\n @-webkit-keyframes newAnim{from{height:50px;\n}\n85%{height:95px;\n}\nto{height:100px;\n}\n}\n @-ms-keyframes newAnim{from{height:50px;\n}\n85%{height:95px;\n}\nto{height:100px;\n}\n}\n @keyframes newAnim{from{height:50px;\n}\n85%{height:95px;\n}\nto{height:100px;\n}\n}\n@media tv and (height >= 200px), (height: 400px) and (300px <= width < 500px){.testClass{text-align:center;\n}\n}\n";
    doc.head.appendChild(style);
  }
  return doc.stateStore;
}