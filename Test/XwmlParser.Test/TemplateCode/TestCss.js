function TestCss_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div class=\"testClass\">Test</div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = [];
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
};
TestCss_var = null;
function TestCss() {
  if (!TestCss_var)
    TestCss_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, TestCss_factory, "0");
  return TestCss_var;
};
tmplStore = new Array(1);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(1);
    style = doc.createElement("style");
    style.textContent = ".testClass[t=ten]>div:hover>#i:nth-child(odd) a:nth-child(2n+1){height:10px;color:rgba(10,20,30,0.4);-webkit-transform:translate3d(10px,40px,10%);-moz-transform:translate3d(10px,40px,10%);-o-transform:translate3d(10px,40px,10%);-ms-transform:translate3d(10px,40px,10%);transform:translate3d(10px,40px,10%);}.testClass{-webkit-animation:newAnim 0.5s ease;-moz-animation:newAnim 0.5s ease;-o-animation:newAnim 0.5s ease;-ms-animation:newAnim 0.5s ease;animation:newAnim 0.5s ease;} @-webkit-keyframes newAnim{from{height:50px;}85%{height:95px;}to{height:100px;}} @-moz-keyframes newAnim{from{height:50px;}85%{height:95px;}to{height:100px;}} @-o-keyframes newAnim{from{height:50px;}85%{height:95px;}to{height:100px;}} @-ms-keyframes newAnim{from{height:50px;}85%{height:95px;}to{height:100px;}} @keyframes newAnim{from{height:50px;}85%{height:95px;}to{height:100px;}}@media tv and (height >= 200px), (height: 400px) and (300px <= width < 500px){.testClass{text-align:center;}}";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};