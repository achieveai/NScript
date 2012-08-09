function RealScript__WhileLoopBlocks__WhilePaddedLoop(i) {
  i %= 100;
  while (i > 10) {
    RealScript__TmpC__Foo("{0}", System_Int32.box(i));
    --i;
  }
  return i;
}
