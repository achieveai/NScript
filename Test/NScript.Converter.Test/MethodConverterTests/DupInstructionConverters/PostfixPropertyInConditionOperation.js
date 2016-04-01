function DupInstructionBlocks__PostfixPropertyInConditionOperation(referenceClass) {
  var returnValue, stmtTemp1;
  returnValue = 0;
  while (referenceClass.set_intProperty((stmtTemp1 = referenceClass.get_intProperty()) + 1), stmtTemp1 == 0)
    returnValue = 2 * returnValue;
  return returnValue;
}