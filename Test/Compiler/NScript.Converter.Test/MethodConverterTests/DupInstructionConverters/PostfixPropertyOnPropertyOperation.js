function DupInstructionBlocks__PostfixPropertyOnPropertyOperation(referenceClass) {
  var stmtTemp1;
  referenceClass.get_property().intField = (referenceClass.get_property().set_intProperty((stmtTemp1 = referenceClass.get_property().get_intProperty()) + 1), stmtTemp1);
}