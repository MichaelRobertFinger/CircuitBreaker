using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public static class CircuitBreakerExtensions
    {
        public static T Retry<T>(this CircuitBreaker circuitBreaker, Func<T> operation, int attemptsUntilTermination)
        {
            Exception exception = new Exception("CircuitBreaker Retry extension encountered an unknown error");
            int terminationCount = 0;

            try
            {
                while (terminationCount < attemptsUntilTermination)
                {
                    try
                    {
                        return circuitBreaker.Execute(operation);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                    terminationCount++;
                }
            }
            catch (Exception ex)
            {
               throw new OperationFailedException(String.Format("The operation terminated after {0} retries.", attemptsUntilTermination), ex);
            }

            throw exception;
        }
    }
}
