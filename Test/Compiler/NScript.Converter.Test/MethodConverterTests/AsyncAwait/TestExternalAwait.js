async function TestAsyncAwait__TestExternalAwait() {
  return await CallContext__WrapPromise(RealScript.JsScriptImport.fetchData("test"));
}