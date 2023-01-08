function GeneratorWrapper__MoveNext(this_) {
  this_._current = this_._generator.next();
  return !this_._current.done;
}
