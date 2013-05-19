function RealScript__WhileLoopBlocks__WhileLoop(i) {
  while (i > 10) {
    RealScript__TmpC__Foo("{0}", System__Type__BoxTypeInstance(System_Int32, i));
    --i;
  }
}