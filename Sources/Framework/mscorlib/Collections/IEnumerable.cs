namespace System.Collections
{
    using System.Threading;

    public interface IEnumerable
    {
        IEnumerator GetEnumerator();
    }

    /*
    public interface IAsyncEnumerable
    {
        IAsyncEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
    */
}

