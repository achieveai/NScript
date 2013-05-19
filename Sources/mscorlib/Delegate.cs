namespace System
{
    using System.Runtime.CompilerServices;

    public abstract class Delegate
    {
        [PreserveCase]
        public static readonly Delegate Null;

        protected extern Delegate(object target, string method);

        protected extern Delegate(Type target, string method);

        [Script(@"
            var i, j, idx, fullMatch, leftArr, rightArr;
            if (left == right)
                return true;
            if (!left || !right)
                return false;
            if (typeof left.funcs == ""undefined"")
                return false;
            if (typeof right.funcs == ""undefined"") {
                return false;
            } else {
                leftArr = left.funcs;
                rightArr = right.funcs;
                if (leftArr.length != rightArr.length) return false;
                for(j = 0; j < leftArr.length; ++j) {
                    if (leftArr[i + j] != rightArr[j]) {
                        return false;
                    }
                }
                return true;
            }
        ")]
        public extern static bool operator == (Delegate left, Delegate right);

        public static bool operator != (Delegate left, Delegate right)
        {
            return !(left == right);
        }

        [Script(@"
            var rv, i, funcs = [];
            if (a != null) {
                if (typeof a.funcs != ""undefined"")
                    funcs = funcs.concat(a.funcs);
                else
                    funcs.push(a);
            }
            if (typeof b.funcs != ""undefined"") {
                funcs = funcs.concat(b.funcs);
            }
            else if (b)
                funcs.push(b);
            else
                return a;
            rv = @{[mscorlib]System.Delegate::CreateJoinedArray([mscorlib]System.NativeArray)}(funcs);
            rv.@{[mscorlib]System.Type::FullName} = 'Multicast Delegate';
            return rv;
        ")]
        public static extern Delegate Combine(Delegate a, Delegate b);

        [Script(@"
            if (!func.@{[mscorlib]System.Type::IsDelegate}) {
                return func.@{[mscorlib]System.Type::IsDelegate} = true;
            }
            return func;
        ")]
        public static extern Delegate Create(Function func);

        [Script(@"
            var rv, func = instance[functionName];
            var fn = ""__@"" + functionName;
            if (!(fn in instance)) {
                instance[fn] = function() {
                    return func.apply(instance, arguments);
                };
                instance[fn].@{[mscorlib]System.Type::FullName} = instance.constructor.toString() + '::' + functionName;
                instance[fn].@{[mscorlib]System.Type::IsDelegate} = true;
            }

            return instance[fn];
        ")]
        internal static extern Delegate Create(
            string functionName,
            object instance);

        [Script(@"
            var fn = ""__@"" + functionName;
            if (!(fn in instance)) {
                instance[fn] = function() {
                    return f.apply(instance, arguments);
                };
                instance[fn].@{[mscorlib]System.Type::FullName} = instance.constructor.toString() + '::' + functionName;
                instance[fn].@{[mscorlib]System.Type::IsDelegate} = true;
            }

            return instance[fn];
        ")]
        internal static extern Delegate StaticInstanceCreate(
            string functionName,
            object instance,
            Function f);

        [Script(@"
            var rv, i, fn = ""__@"" + functionName;
            for(i = 0; i < genericArguments.length; ++i) {
                fn = fn + ""_@"" + genericArguments[i].@{[mscorlib]System.Type::TypeId} + ""@"";
            }
            if (!(fn in instanceOrType)) {
                instanceOrType[fn] = function() {
                    return f.apply(instanceOrType, genericArguments.concat(Array.prototype.slice.call(arguments)));
                };
                instanceOrType[fn].@{[mscorlib]System.Type::FullName} = instanceOrType.constructor.toString() + '::' + functionName;
                instanceOrType[fn].@{[mscorlib]System.Type::IsDelegate} = true;
            }

            return instanceOrType[fn];
        ")]
        internal static extern Delegate CreateGeneric(
            string functionName,
            object instanceOrType,
            Function f,
            NativeArray genericArguments);

        [Script(@"
            return @{[mscorlib]System.Delegate::CreateGeneric([mscorlib]System.String, [mscorlib]System.Object, [mscorlib]System.Function, [mscorlib]System.NativeArray)}(
                functionName,
                instance,
                instance[functionName],
                genericArguments);
            ")]
        internal static extern Delegate CreateInstanceGeneric(
            string functionName,
            object instance,
            NativeArray genericArguments);

        [Script(@"
            var rv, i, j, idx, fullMatch, valArr;
            if (source == value)
                return null;
            if (typeof source.funcs == ""undefined"")
                return source;
            var newArr = [].concat(source.funcs);
            if (typeof value.funcs != ""undefined"") {
                valArr= value.funcs;
                for(i = newArr.length - valArr.length; i >= 0; --i) {
                    fullMatch = true;
                    for(j = 0; j < valArr.length; ++j) {
                        if (newArr[i + j] != valArr[j]) {
                            fullMatch = false;
                            break;
                        }
                    }

                    if (fullMatch) {
                        newArr.splice(i, valArr.length);
                        break;
                    }
                }
            } else {
                for(i = newArr.length -1; i >= 0; --i) {
                    if (newArr[i] === value) {
                        newArr.splice(i, 1);
                        break;
                    }
                }
            }

            if (newArr.length == 0)
                return null;
            else if (newArr.length == 1)
                return newArr[0];
            else if (newArr.length == source.funcs.length)
                return source;
            rv = @{[mscorlib]System.Delegate::CreateJoinedArray([mscorlib]System.NativeArray)}(newArr);
            rv.@{[mscorlib]System.Type::FullName} = 'Multicast Delegate';
            return rv;
        ")]
        public static extern Delegate Remove(Delegate source, Delegate value);

        [Script(@"
            var i, rv = function() {
                var rv1;
                for(i = 0; i < array.length; ++i) rv1 = array[i].apply(null, arguments);
                return rv1;
            };
            rv.funcs = array;
            rv.@{[mscorlib]System.Type::IsDelegate} = true;
            return rv;
        ")]
        private static extern Delegate CreateJoinedArray(NativeArray array);
    }
}

