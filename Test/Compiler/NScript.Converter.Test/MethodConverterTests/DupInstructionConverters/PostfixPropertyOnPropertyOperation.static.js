function DupInstructionBlocks__PostfixPropertyOnPropertyOperation(referenceClass) {
  var stmtTemp1;
  TestReferenceClass__get_Property(referenceClass).intField = (TestReferenceClass__set_IntProperty(TestReferenceClass__get_Property(referenceClass), (stmtTemp1 = TestReferenceClass__get_IntProperty(TestReferenceClass__get_Property(referenceClass))) + 1), stmtTemp1);
}