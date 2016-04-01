function DupInstructionBlocks__PostfixPropertyOperation(referenceClass) {
  var stmtTemp1;
  referenceClass.intField = (referenceClass.set_intProperty((stmtTemp1 = referenceClass.get_intProperty()) + 1), stmtTemp1);
}