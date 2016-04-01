function DelegateBlocks__LocalAndInstanceReferencingDelegate(j) {
  var this_;
  this_ = this;
  return function DelegateBlocks__LocalAndInstanceReferencingDelegate_del(i) {
    return i + j + this_.intVariable;
  };
}