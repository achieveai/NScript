using System.Runtime.CompilerServices;
using System.Collections;

namespace System
{
    public class GeneratorWrapper: IEnumerator, IEnumerable
    {
        private NativeGenerator _generator;
        private NativeGenerator.NextObject _current;

        [Script("this.@{[mscorlib]System.GeneratorWrapper::_generator} = generatorFunction();")]
        public extern GeneratorWrapper(object generatorFunction);

        object IEnumerator.Current => _current.Value;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        bool IEnumerator.MoveNext()
        {
            this._current = this._generator.Next();
            return !this._current.Done;
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
