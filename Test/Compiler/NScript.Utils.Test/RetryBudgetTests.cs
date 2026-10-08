namespace NScript.Utils.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Lib.Service;

    /// <summary>
    /// Defect D4 (M5): while TodoApp.dll stayed locked, our own bundle writes started batches
    /// that each retried the copy and scheduled another timer, so the 5 retries were spent in
    /// 2.4 s instead of 5 s, and a leftover timer restarted the count at 1/5 after giving up.
    /// </summary>
    [TestClass]
    public class RetryBudgetTests
    {
        private const string Key = @"B:\app\bin\TodoApp.dll";

        [TestMethod]
        public void OnFailure_WhileRetryScheduled_SpendsNothingAndAddsNoTimer()
        {
            var budget = new RetryBudget(5);
            Assert.AreEqual(RetryDecision.Schedule, budget.OnFailure(Key, out int retry));
            Assert.AreEqual(1, retry);

            for (int unrelatedBatch = 0; unrelatedBatch < 4; unrelatedBatch++)
            {
                Assert.AreEqual(RetryDecision.AlreadyScheduled, budget.OnFailure(Key, out _));
            }

            budget.TimerFired(Key);
            Assert.AreEqual(RetryDecision.Schedule, budget.OnFailure(Key, out retry));
            Assert.AreEqual(2, retry, "Only timed retries spend the budget.");
        }

        [TestMethod]
        public void GiveUp_StaysQuietUntilARealChange_ThenAFreshBudget()
        {
            var budget = new RetryBudget(2);
            for (int expected = 1; expected <= 2; expected++)
            {
                Assert.AreEqual(RetryDecision.Schedule, budget.OnFailure(Key, out int retry));
                Assert.AreEqual(expected, retry);
                budget.TimerFired(Key);
            }

            Assert.AreEqual(RetryDecision.GiveUp, budget.OnFailure(Key, out _));
            Assert.AreEqual(RetryDecision.GivenUp, budget.OnFailure(Key, out _), "A later batch must not restart the retries.");

            budget.Renew();
            Assert.AreEqual(RetryDecision.Schedule, budget.OnFailure(Key, out int fresh));
            Assert.AreEqual(1, fresh);

            budget.TimerFired(Key);
            budget.Succeeded(Key);
            Assert.AreEqual(RetryDecision.Schedule, budget.OnFailure(Key, out fresh));
            Assert.AreEqual(1, fresh, "A success resets the budget.");
        }
    }
}
