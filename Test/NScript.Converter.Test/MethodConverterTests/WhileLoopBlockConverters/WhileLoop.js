function WhileLoopBlocks__WhileLoop(i) {
  while (i > 10) {
    TmpC__Foo("{0}", Type__BoxTypeInstance(Int32, i));
    --i;
  }
}