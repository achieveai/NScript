namespace System.Threading
{
    //
    // Summary:
    //     Signals to a System.Threading.CancellationToken that it should be canceled.
    public class CancellationTokenSource : IDisposable
    {
        //
        // Summary:
        //     Gets the System.Threading.CancellationToken associated with this System.Threading.CancellationTokenSource.
        //
        // Returns:
        //     The System.Threading.CancellationToken associated with this System.Threading.CancellationTokenSource.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The token source has been disposed.
        public CancellationToken Token { get; }

        //
        // Summary:
        //     Creates a System.Threading.CancellationTokenSource that will be in the canceled
        //     state when the supplied token is in the canceled state.
        //
        // Parameters:
        //   token:
        //     The cancellation token to observe.
        //
        // Returns:
        //     An object that's linked to the source token.
        public static extern CancellationTokenSource CreateLinkedTokenSource(CancellationToken token);
        //
        // Summary:
        //     Creates a System.Threading.CancellationTokenSource that will be in the canceled
        //     state when any of the source tokens are in the canceled state.
        //
        // Parameters:
        //   token1:
        //     The first cancellation token to observe.
        //
        //   token2:
        //     The second cancellation token to observe.
        //
        // Returns:
        //     A System.Threading.CancellationTokenSource that is linked to the source tokens.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     A System.Threading.CancellationTokenSource associated with one of the source
        //     tokens has been disposed.
        public static extern CancellationTokenSource CreateLinkedTokenSource(CancellationToken token1, CancellationToken token2);
        //
        // Summary:
        //     Creates a System.Threading.CancellationTokenSource that will be in the canceled
        //     state when any of the source tokens in the specified array are in the canceled
        //     state.
        //
        // Parameters:
        //   tokens:
        //     An array that contains the cancellation token instances to observe.
        //
        // Returns:
        //     A System.Threading.CancellationTokenSource that is linked to the source tokens.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     A System.Threading.CancellationTokenSource associated with one of the source
        //     tokens has been disposed.
        //
        //   T:System.ArgumentNullException:
        //     tokens is null.
        //
        //   T:System.ArgumentException:
        //     tokens is empty.
        public static extern CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens);
        //
        // Summary:
        //     Releases all resources used by the current instance of the System.Threading.CancellationTokenSource
        //     class.
        public extern void Dispose();
    }
}
