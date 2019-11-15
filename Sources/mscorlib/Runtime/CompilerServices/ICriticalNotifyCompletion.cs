//-----------------------------------------------------------------------
// <copyright file="TaskAwaiter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    //
    // Summary:
    //     Represents an awaiter that schedules continuations when an await operation completes.
    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        //
        // Summary:
        //     Schedules the continuation action that's invoked when the instance completes.
        //
        // Parameters:
        //   continuation:
        //     The action to invoke when the operation completes.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The continuation argument is null (Nothing in Visual Basic).
        void UnsafeOnCompleted(Action continuation);
    }
}
