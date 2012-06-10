function RealScript__DupInstructionBlocks__PostfixSecondOrderPropertyOperation(referenceClass) {
  var stmtTemp1;
  referenceClass.intField = (referenceClass.get_property().set_intProperty((stmtTemp1 = referenceClass.get_property().get_intProperty()) + 1), stmtTemp1);
}