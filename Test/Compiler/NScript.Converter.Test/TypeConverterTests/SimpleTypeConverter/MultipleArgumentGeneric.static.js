function MultiArgGeneric(T, U, _callStatiConstructor) {
  var MultiArgGeneric$2_$T_x_U$_;
  if (MultiArgGeneric["9" + T.typeId] && MultiArgGeneric["9" + T.typeId][U.typeId])
    return MultiArgGeneric["9" + T.typeId][U.typeId];
    MultiArgGeneric["9" + T.typeId] = {
    };
  MultiArgGeneric["9" + T.typeId][U.typeId] = MultiArgGeneric$2_$T_x_U$_ = function RealScript__MultiArgGeneric$2() {
  };
  MultiArgGeneric$2_$T_x_U$_.genericParameters = [T, U];
  MultiArgGeneric$2_$T_x_U$_.genericClosure = MultiArgGeneric;
  MultiArgGeneric$2_$T_x_U$_.typeId = "b$" + T.typeId + "_" + U.typeId + "$";
  MultiArgGeneric$2_$T_x_U$_.t = Type__GetDefaultValueStatic(T);
  MultiArgGeneric$2_$T_x_U$_.u = Type__GetDefaultValueStatic(U);
  MultiArgGeneric$2_$T_x_U$_.defaultConstructor = function RealScript_MultiArgGeneric$2_factory() {
    var this_;
    this_ = new MultiArgGeneric$2_$T_x_U$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = MultiArgGeneric$2_$T_x_U$_.prototype;
  ptyp_.__ctor = function MultiArgGeneric$2____ctor() {
  };
  ptyp_.getValue = function MultiArgGeneric$2__GetValue() {
    return MultiArgGeneric$2_$T_x_U$_.t.toString() + MultiArgGeneric$2_$T_x_U$_.u.toString();
  };
  Type__RegisterReferenceType(MultiArgGeneric$2_$T_x_U$_, "RealScript.MultiArgGeneric`2<" + T.fullName + "," + U.fullName + ">", Object, []);
  return MultiArgGeneric$2_$T_x_U$_;
};
function MultiArgGenerica(T, U, V, _callStatiConstructor) {
  var MultiArgGeneric$2_$T_x_U$_, MultiArgGeneric$3_$T_x_U_x_V$_;
  if (MultiArgGenerica["9" + T.typeId] && MultiArgGenerica["9" + T.typeId][U.typeId] && MultiArgGenerica["9" + T.typeId][U.typeId][V.typeId])
    return MultiArgGenerica["9" + T.typeId][U.typeId][V.typeId];
    MultiArgGenerica["9" + T.typeId] = {
    };
    MultiArgGenerica["9" + T.typeId][U.typeId] = {
    };
  MultiArgGenerica["9" + T.typeId][U.typeId][V.typeId] = MultiArgGeneric$3_$T_x_U_x_V$_ = function RealScript__MultiArgGeneric$3() {
  };
  MultiArgGeneric$3_$T_x_U_x_V$_.genericParameters = [T, U, V];
  MultiArgGeneric$3_$T_x_U_x_V$_.genericClosure = MultiArgGenerica;
  MultiArgGeneric$3_$T_x_U_x_V$_.typeId = "c$" + T.typeId + "_" + U.typeId + "_" + V.typeId + "$";
  MultiArgGeneric$2_$T_x_U$_ = MultiArgGeneric(T, U, _callStatiConstructor);
  MultiArgGeneric$3_$T_x_U_x_V$_.v = Type__GetDefaultValueStatic(V);
  MultiArgGeneric$3_$T_x_U_x_V$_.defaultConstructor = function RealScript_MultiArgGeneric$3_factory() {
    var this_;
    this_ = new MultiArgGeneric$3_$T_x_U_x_V$_();
    this_.__ctora();
    return this_;
  };
  ptyp_ = new MultiArgGeneric$2_$T_x_U$_();
  MultiArgGeneric$3_$T_x_U_x_V$_.prototype = ptyp_;
  ptyp_.__ctora = function MultiArgGeneric$3____ctor() {
    this.__ctor();
  };
  ptyp_.getValuea = function MultiArgGeneric$3__GetValue() {
    return this.getValue() + MultiArgGeneric$3_$T_x_U_x_V$_.v.toString();
  };
  Type__RegisterReferenceType(MultiArgGeneric$3_$T_x_U_x_V$_, "RealScript.MultiArgGeneric`3<" + T.fullName + "," + U.fullName + "," + V.fullName + ">", MultiArgGeneric$2_$T_x_U$_, []);
  return MultiArgGeneric$3_$T_x_U_x_V$_;
};