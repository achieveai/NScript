function RealScript__WhileLoopBlocks__WhilePaddedLoop(i) {
  i %= 100;
  while (i > 10) {
    RealScript__TmpC__Foo("{0}", System__Type__BoxTypeInstance(System_Int32, i));
    --i;
  }
  return i;
}
