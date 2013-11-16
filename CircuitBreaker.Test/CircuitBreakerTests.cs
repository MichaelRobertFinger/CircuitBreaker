using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CircuitBreaker.Test
{
    [TestClass]
    public class CircuitBreakerTests
    {
        [TestMethod]
        public void Constructor_Empty()
        {
            var cb = new CircuitBreaker();

            Assert.AreEqual(1000, cb.Timeout);
            Assert.AreEqual(5, cb.Threshold);
            Assert.AreEqual(CircuitBreakerState.Closed, cb.State);
        }

        [TestMethod]
        public void Constructor_Overload()
        {
            var cb = new CircuitBreaker(1500, 2);

            Assert.AreEqual(1500, cb.Timeout);
            Assert.AreEqual(2, cb.Threshold);
            Assert.AreEqual(CircuitBreakerState.Closed, cb.State);
        }

        [TestMethod]
        public void CanChangeTimeout()
        {
            var expected = 66;
            var cb = new CircuitBreaker(100, 5);
            cb.Timeout = expected;

            Assert.AreEqual(expected, cb.Timeout);
        }

        [TestMethod]
        public void CanChangeThreshold()
        {
            var expected = 66;
            var cb = new CircuitBreaker(100, 5);
            cb.Threshold = expected;

            Assert.AreEqual(expected, cb.Threshold);
        }

        [TestMethod]
        public void Trip_ChangesStateToOpen()
        {
            var cb = new CircuitBreaker();
            cb.Trip();

            Assert.AreEqual(CircuitBreakerState.Open, cb.State);
        }

        [TestMethod]
        public void Trip_StartsTimer()
        {
            var cb = new CircuitBreaker(250, 2);
            cb.Trip();

            Thread.Sleep(500);

            Assert.AreEqual(CircuitBreakerState.HalfOpen, cb.State);
        }

        [TestMethod]
        public void Reset_ChangesStateToClosed()
        {
            var cb = new CircuitBreaker();
            cb.Trip();
            cb.Reset();

            Assert.AreEqual(CircuitBreakerState.Closed, cb.State);
        }

        [TestMethod]
        public void Timeout_ChangeIncreasesTimerInterval()
        {
            var cb = new CircuitBreaker(500, 3);
            cb.Trip();
            cb.Timeout = 8000;

            // if Timeout does not update _timer.Interval, timer_Elapsed method would be called and state changed to halfopen.
            Thread.Sleep(1000);

            Assert.AreNotEqual(CircuitBreakerState.HalfOpen, cb.State);
        }

        [TestMethod]
        [ExpectedException(typeof(OpenCircuitException))]
        public void Execute_OpenStateThrowsException()
        {
            var cb = new CircuitBreaker();
            cb.Trip();
            cb.Execute(new Func<int>(() => { return 1; }));
        }

        //[TestMethod]
        //public void Execute


        /// <summary>
        /// test to continue to run circuit breaker until absolute fail.
        /// attemptsCount is total of all attempts to run the cb.Execute method.
        /// </summary>
        [Ignore]
        [TestMethod]
        public void CircuitBreaker_ConstructorOverload()
        {
            Func<int> func = new Func<int>(() => { throw new Exception(); });
            var cb = new CircuitBreaker(500, 3);
            object result = null;
            var totalAttempts = 5;
            var attemptsCount = 0;
            while (result == null && attemptsCount < totalAttempts)
            {
                if (cb.State != CircuitBreakerState.Open)
                {
                    result = cb.Execute(func);
                    attemptsCount++;
                }
            }

            Assert.AreEqual(CircuitBreakerState.Open, cb.State);
        }

        [Ignore]
        [TestMethod]
        public void Execute_CanExecuteOperation()
        {
            Func<int> func = new Func<int>(() => { return 1 + 2; });
            var cb = new CircuitBreaker(10000, 3);
            var result = cb.Execute(func);

            Assert.AreEqual(3, result);
        }
    }
}
