using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitBreaker
{
    public class OperationFailedException : Exception
    {
        public OperationFailedException(string message, Exception ex)
            : base(message, ex)
        { }
    }
}
