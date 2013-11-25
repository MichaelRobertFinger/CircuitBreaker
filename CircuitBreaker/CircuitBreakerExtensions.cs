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
            List<Exception> _exceptions = new List<Exception>();
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
                        _exceptions.Add(ex);
                    }

                    terminationCount++;
                }
            }
            catch (Exception ex)
            {
                throw new AggregateException("The operation terminated due to an unknown error.", ex);
            }

            throw new AggregateException(String.Format("The operation terminated after {0} retries.", attemptsUntilTermination), _exceptions);
        }
    }
}
