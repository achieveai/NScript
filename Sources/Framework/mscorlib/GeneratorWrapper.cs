using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    public class GeneratorWrapper<T> : IEnumerator<T>, IEnumerable<T>
    {
        private NativeGenerator _generator;
        private NativeGenerator.NextObject _current;

        [Script("this.@{[mscorlib]System.GeneratorWrapper::_generator} = generatorFunction();")]
        public extern GeneratorWrapper(object generatorFunction);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        object IEnumerator.Current => _current.Value;
        
        public T Current
        {
            [Script("this.@{[mscorlib]System.GeneratorWrapper::_current}.value")]
            get;
        }

        public bool MoveNext()
        {
            this._current = this._generator.Next();
            return !this._current.Done;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
