using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CircuitBreaker.Test
{
    [TestClass]
    public class CircuitBreakerTests
    {
        [TestMethod]
        public void CircuitBreaker_EmptyConstructor()
        {
            var cb = new CircuitBreaker();

            Assert.AreEqual(1000, cb.Timeout);
            Assert.AreEqual(5, cb.Threshold);
            Assert.AreEqual(CircuitBreakerState.Closed, cb.State);
        }

        [TestMethod]
        public void CircuitBreaker_Constructor()
        {
            var cb = new CircuitBreaker(1500, 2);

            Assert.AreEqual(1500, cb.Timeout);
            Assert.AreEqual(2, cb.Threshold);
            Assert.AreEqual(CircuitBreakerState.Closed, cb.State);
        }

        /// <summary>
        /// test to continue to run circuit breaker until absolute fail.
        /// attemptsCount is total of all attempts to run the cb.Execute method.
        /// </summary>
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

        public void Execute_CanExecuteOperation()
        {
            Func<int> func = new Func<int>(() => { return 1 + 2; });
            var cb = new CircuitBreaker(10000, 3);
            var result = cb.Execute(func);

            Assert.AreEqual(3, result);
        }
    }
}
