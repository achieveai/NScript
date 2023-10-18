namespace System
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncGeneratorWrapper<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
    {
        private NativeAsyncGenerator _generator;
        private NativeAsyncGenerator.NextObject _current;

        [Script("this.@{[mscorlib]System.AsyncGeneratorWrapper`1::_generator} = generatorFunction();")]
        public extern AsyncGeneratorWrapper(object generatorFunction);

        public T Current
        {
            get => Type.AS<object, T>(_current.Value);
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            _current = await _generator.NextAsync();
            return !_current.Done;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _)
        {
            return this;
        }
    }
}
