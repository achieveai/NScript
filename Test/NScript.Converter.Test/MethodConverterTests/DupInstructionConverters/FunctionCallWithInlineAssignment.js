function DupInstructionBlocks__FunctionCallWithInlineAssignment(i, cl1) {
  return cl1.func2(i = cl1.foo1(), i + 10);
}
