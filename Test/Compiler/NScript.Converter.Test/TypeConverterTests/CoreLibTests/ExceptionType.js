function NotImplementedException() { }
NotImplementedException.typeId = "b";
function NotImplementedException_factory() {
  var this_;
  this_ = new NotImplementedException();
  this_.__ctor();
  return this_;
}
NotImplementedException.defaultConstructor = NotImplementedException_factory;
function NotImplementedException_factorya(message) {
  var this_;
  this_ = new NotImplementedException();
  this_.__ctora(message);
  return this_;
}
ptyp_ = Object.create(Error.prototype);
NotImplementedException.prototype = ptyp_;
ptyp_.__ctor = function NotImplementedException____ctor() { };
ptyp_.__ctora = function NotImplementedException____ctora(message) { };
Type__RegisterReferenceType(NotImplementedException, "System.NotImplementedException", Error, []);