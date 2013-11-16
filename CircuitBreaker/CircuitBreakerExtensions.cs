using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerExtensions
    {
        public static T Retry<T>(this CircuitBreaker circuitBreaker, Func<T> operation, int attemptsUntilTermination)
        {
            T result;

            try
            {
                result = circuitBreaker.Execute(operation);
            }
            catch (Exception ex)
            {
                throw new OperationFailedException(String.Format("The operation terminated after {0} retries.", attemptsUntilTermination), ex);
            }

            return result;
        }
    }
}
