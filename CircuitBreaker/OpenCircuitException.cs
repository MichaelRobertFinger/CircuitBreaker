using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitBreaker
{
    public class OpenCircuitException : Exception
    {
        public OpenCircuitException(string message) : base(message)
        {
        }
    }
}
