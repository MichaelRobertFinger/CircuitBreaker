using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CircuitBreaker.Test
{
    [TestClass]
    public class CircuitBreakerExtensionTests
    {
        [TestMethod]
        public void Retry_Failure()
        {
            AggregateException exceptions = new AggregateException();

            try
            {
                var func = new Func<int>(() => { throw new Exception(); });
                var cb = new CircuitBreaker(500, 1);
                cb.Retry<int>(func, 2);
            }
            catch (AggregateException ex)
            {
                exceptions = ex;
            }

            Assert.AreEqual("The operation terminated after 2 retries.", exceptions.Message);
            Assert.AreEqual(2 , exceptions.InnerExceptions.Count);
        }

        [TestMethod]
        public void Retry_WithZeroAttempts_Failure()
        {
            AggregateException exceptions = new AggregateException();

            try
            {
                var func = new Func<int>(() => { throw new Exception(); });
                var cb = new CircuitBreaker(500, 1);
                cb.Retry<int>(func, 0);
            }
            catch (AggregateException ex)
            {
                exceptions = ex;
            }

            Assert.AreEqual("The operation terminated after 0 retries.", exceptions.Message);
            Assert.AreEqual(0, exceptions.InnerExceptions.Count);
        }
    }
}
