function RealScript__DelegateBlocks__LocalAndInstanceReferencingDelegate(j) {
  var this_;
  this_ = this;
  return function RealScript__DelegateBlocks__LocalAndInstanceReferencingDelegate_del(i) {
    return i + j + this_.intVariable;
  };
}