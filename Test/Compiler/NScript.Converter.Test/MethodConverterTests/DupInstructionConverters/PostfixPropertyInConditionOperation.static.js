function DupInstructionBlocks__PostfixPropertyInConditionOperation(referenceClass) {
  var returnValue, stmtTemp1;
  returnValue = 0;
  while (0 == (TestReferenceClass__set_IntProperty(referenceClass, (stmtTemp1 = TestReferenceClass__get_IntProperty(referenceClass)) + 1), stmtTemp1))
    returnValue = 2 * returnValue;
  return returnValue;
}