function RealScript__WhileLoopBlocks__DoWhileLoop(i) {
  do {
    RealScript__TmpC__Foo("{0}", System__Type__BoxTypeInstance(System_Int32, i));
    --i;
  } while (i > 10);
}