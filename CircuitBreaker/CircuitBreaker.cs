using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private int _failureCount;
        private CircuitBreakerState _state;
        private int _timeout;
        private int _threshold;
        private System.Timers.Timer _timer;

        public CircuitBreaker()
            : this(1000, 5)
        { }

        public CircuitBreaker(int timeout, int threshold)
        {
            _threshold = threshold;
            _state = CircuitBreakerState.Closed;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _state = CircuitBreakerState.HalfOpen;
            _timer.Stop();
        }

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int Timeout
        {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = _timeout; }
        }

        public int Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        public CircuitBreakerState State
        {
            get { return _state; }
        }

        public object Execute(Delegate action, params object[] args)
        {
            object result = null;
            try
            {
                result = action.DynamicInvoke(args);
            }
            catch (Exception ex)
            {
                if (_state == CircuitBreakerState.HalfOpen)
                {
                    // Operation failed in a half-open state, so reopen circuit
                    Trip();
                }
                else if (_failureCount < _threshold)
                {
                    _failureCount++;
                }
                else if (_failureCount >= _threshold)
                {
                    // Failure count has reached threshold, so trip circuit breaker
                    Trip();
                }
            }

            if (_state == CircuitBreakerState.HalfOpen)
            {
                Reset();
            }

            return result;
        }

        public void Trip()
        {
            _state = CircuitBreakerState.Open;
            _timer.Start();
        }

        public void Reset()
        {
            _state = CircuitBreakerState.Closed;
            _timer.Stop();
        }
    }
}
