using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class AsyncGeneratorWrapper : IAsyncEnumerator, IAsyncEnumerable
    {
        private NativeAsyncGenerator _generator;
        private NativeAsyncGenerator.NextObject _current;

        [Script("this.@{[mscorlib]System.GeneratorWrapper::_generator} = generatorFunction();")]
        public extern AsyncGeneratorWrapper(object generatorFunction);

        object IAsyncEnumerator.Current => _current.Value;

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator(
            CancellationToken _)
        {
            return this;
        }

        async ValueTask<bool> IAsyncEnumerator.MoveNextAsync()
        {
            _current = await _generator.NextAsync();
            return !_current.Done;
        }
    }

    public class AsyncGeneratorWrapper<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
    {
        private NativeAsyncGenerator _generator;
        private NativeAsyncGenerator.NextObject _current;

        [Script("this.@{[mscorlib]System.GeneratorWrapper`1::_generator} = generatorFunction();")]
        public extern AsyncGeneratorWrapper(object generatorFunction);

        public extern T Current
        {
            [Script("this.@{[mscorlib]System.AsyncGeneratorWrapper`1::_current}.value")]
            get;
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            _current = await _generator.NextAsync();
            return _current.Done;
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(
            CancellationToken _)
        {
            return this;
        }
    }
}
