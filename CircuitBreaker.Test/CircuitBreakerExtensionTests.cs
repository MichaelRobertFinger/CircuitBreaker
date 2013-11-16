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
        [ExpectedException(typeof(OperationFailedException))]
        public void Retry_Failure()
        {
            var func = new Func<int>(() => { throw new Exception(); });
            var cb = new CircuitBreaker(500, 1);
            cb.Retry<int>(func, 2);


        }
    }
}
