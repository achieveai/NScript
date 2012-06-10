function RealScript__ExceptionHandlerSamples__TryCatchFinallySimple(arg) {
  var stmtTemp1;
  arg = RealScript__Class1__GetMoreStatic(arg);
  try {
    arg = RealScript__Class1__GetMoreStatic(arg);
  } catch (stmtTemp1) {
    RealScript__Class1__GetMoreStatic(arg);
  } finally {
    arg -= 2;
  }
  return arg - 10;
}
