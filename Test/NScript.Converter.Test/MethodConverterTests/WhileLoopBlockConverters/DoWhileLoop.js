function WhileLoopBlocks__DoWhileLoop(i) {
  do {
    TmpC__Foo("{0}", Type__BoxTypeInstance(Int32, i));
    --i;
  } while (i > 10);
}