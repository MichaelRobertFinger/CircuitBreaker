using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitBreaker
{
    public enum CircuitBreakerState
    {
        Closed = 0,
        Open = 1,
        HalfOpen = 2,
    }
}
