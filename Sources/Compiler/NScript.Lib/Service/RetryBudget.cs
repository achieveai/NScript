namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;

    /// <summary>What to do after a transient failure (a locked file) of one key.</summary>
    public enum RetryDecision
    {
        /// <summary>Schedule one timed retry.</summary>
        Schedule,

        /// <summary>A timed retry is already scheduled; this failure came from another batch.</summary>
        AlreadyScheduled,

        /// <summary>The last timed retry failed: wait for a real change. Reported once.</summary>
        GiveUp,

        /// <summary>Already given up; stay quiet until a real change or a success.</summary>
        GivenUp,
    }

    /// <summary>
    /// Timed retries of transient failures (a locked copy target, an unreadable reference, a
    /// bundle held open), per key. At most one timer is outstanding per key, so failures in
    /// batches started by other events (our own outputs, other saves) neither add timers nor
    /// spend the budget. After <see cref="MaxRetries"/> failed timed retries the key gives up
    /// and stays quiet until <see cref="Renew"/> (a real change) or a success.
    /// Not thread safe: the watch host calls it under its watch lock.
    /// </summary>
    public sealed class RetryBudget
    {
        private readonly Dictionary<string, State> states = new Dictionary<string, State>(StringComparer.OrdinalIgnoreCase);

        public RetryBudget(int maxRetries)
        {
            this.MaxRetries = maxRetries;
        }

        public int MaxRetries { get; }

        /// <summary>
        /// Records a failure of <paramref name="key"/>. On <see cref="RetryDecision.Schedule"/>
        /// the caller schedules one timer and calls <see cref="TimerFired"/> when it fires;
        /// <paramref name="retry"/> is that retry's number (1-based), else the retries spent.
        /// </summary>
        public RetryDecision OnFailure(string key, out int retry)
        {
            if (!this.states.TryGetValue(key, out var state))
            {
                state = new State();
                this.states[key] = state;
            }

            retry = state.Retries;
            if (state.GivenUp)
            {
                return RetryDecision.GivenUp;
            }

            if (state.TimerPending)
            {
                return RetryDecision.AlreadyScheduled;
            }

            if (state.Retries >= this.MaxRetries)
            {
                state.GivenUp = true;
                return RetryDecision.GiveUp;
            }

            state.Retries++;
            state.TimerPending = true;
            retry = state.Retries;
            return RetryDecision.Schedule;
        }

        /// <summary>The timer of <paramref name="key"/> fired; the batch it starts is the retry.</summary>
        public void TimerFired(string key)
        {
            if (this.states.TryGetValue(key, out var state))
            {
                state.TimerPending = false;
            }
        }

        /// <summary>The key succeeded: its next failure starts a fresh budget.</summary>
        public void Succeeded(string key) => this.Reset(key);

        /// <summary>A real change (a save) arrived: every key gets a fresh budget. Pending timers stay.</summary>
        public void Renew()
        {
            foreach (var key in this.states.Keys)
            {
                this.Reset(key);
            }
        }

        private void Reset(string key)
        {
            if (this.states.TryGetValue(key, out var state))
            {
                state.Retries = 0;
                state.GivenUp = false;
            }
        }

        private sealed class State
        {
            public int Retries;

            public bool TimerPending;

            public bool GivenUp;
        }
    }
}
