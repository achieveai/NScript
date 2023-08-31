namespace System.Threading
{
    //
    // Summary:
    //     Propagates notification that operations should be canceled.
    public readonly struct CancellationToken : IEquatable<CancellationToken>
    {
        //
        // Summary:
        //     Initializes the System.Threading.CancellationToken.
        //
        // Parameters:
        //   canceled:
        //     The canceled state for the token.
        public extern CancellationToken(bool canceled);

        //
        // Summary:
        //     Determines whether the current System.Threading.CancellationToken instance is
        //     equal to the specified token.
        //
        // Parameters:
        //   other:
        //     The other System.Threading.CancellationToken to compare with this instance.
        //
        // Returns:
        //     true if the instances are equal; otherwise, false. See the Remarks section for
        //     more information.
        public extern bool Equals(CancellationToken other);
    }
}
