function DupInstructionBlocks__InlineOpAssignment(trc1) {
  trc1.intField = TestReferenceClass__set_IntProperty(trc1, TestReferenceClass__get_IntProperty(trc1) + 10);
}
