function DupInstructionBlocks__PropertyInlineEquals(referenceClass) {
  referenceClass.intField = referenceClass.set_intProperty(referenceClass.get_property().intField);
}
