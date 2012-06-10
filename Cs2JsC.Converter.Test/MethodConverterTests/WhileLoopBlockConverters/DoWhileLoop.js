function RealScript__WhileLoopBlocks__DoWhileLoop(i) {
  do {
    RealScript__TmpC__Foo("{0}", System_Int32.box(i));
    --i;
  } while (i > 10);
}
