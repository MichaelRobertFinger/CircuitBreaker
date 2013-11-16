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
        private int _threshold;
        private System.Timers.Timer _timer;

        public event EventHandler StateChanged;

        public CircuitBreaker()
            : this(1000, 5)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">in milliseconds</param>
        /// <param name="threshold"></param>
        /// <param name="terminateThreshold"></param>
        public CircuitBreaker(int timeout, int threshold)
        {
            _threshold = threshold;
            _state = CircuitBreakerState.Closed;
            _timer = new System.Timers.Timer(timeout);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int Timeout
        {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = value; }
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

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            if (_state == CircuitBreakerState.Open)
                throw new OpenCircuitException("Circuit breaker is currently open");

            TResult result;

            try 
            {
                // Execute operation
                result = operation();
            }
            catch (Exception ex)
            {
                //ChangeState(CircuitBreakerState.Open);
                throw new OperationFailedException("Operation failed", ex);
            }

            return result;
        }

        public void Trip()
        {
            ChangeState(CircuitBreakerState.Open);
            _timer.Start();
        }

        public void Reset()
        {
            ChangeState(CircuitBreakerState.Closed);
            _timer.Stop();
        }

        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        private void ChangeState(CircuitBreakerState state)
        {
            _state = state;
            OnStateChanged(new EventArgs());
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChangeState(CircuitBreakerState.HalfOpen);
            _timer.Stop();
        }
    }
}
