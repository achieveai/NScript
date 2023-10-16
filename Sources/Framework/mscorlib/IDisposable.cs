namespace System
{
    using System.Threading.Tasks;

    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }

    public interface IDisposable
    {
        void Dispose();
    }
}