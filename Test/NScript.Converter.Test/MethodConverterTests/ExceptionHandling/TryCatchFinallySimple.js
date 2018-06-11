function ExceptionHandlerSamples__TryCatchFinallySimple(arg) {
  var stmtTemp1;
  arg = Class1__GetMoreStatic(arg);
  try {
    arg = Class1__GetMoreStatic(arg);
  } catch (stmtTemp1) {
    Class1__GetMoreStatic(arg);
  } finally {
    arg -= 2;
  }
  return arg - 10;
}
