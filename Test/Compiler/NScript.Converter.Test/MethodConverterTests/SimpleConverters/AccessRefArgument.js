function BasicStatements__AccessRefArgument(i1, testOut) {
  testOut.write(i1 + 10 + testOut.read());
  return i1;
}
