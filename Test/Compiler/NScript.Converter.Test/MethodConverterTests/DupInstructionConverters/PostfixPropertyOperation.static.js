function DupInstructionBlocks__PostfixPropertyOperation(referenceClass) {
  var stmtTemp1;
  referenceClass.intField = (TestReferenceClass__set_IntProperty(referenceClass, (stmtTemp1 = TestReferenceClass__get_IntProperty(referenceClass)) + 1), stmtTemp1);
}