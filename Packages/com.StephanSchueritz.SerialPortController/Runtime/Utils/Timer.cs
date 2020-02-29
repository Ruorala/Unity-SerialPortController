using UnityEngine;

namespace Utils
{
    public class LoopTimer
    {
        public LoopTimer(float newTime)
        {
            time = newTime;
            currentTime = newTime;
        }

        public LoopTimer(float time, System.Action onTrigger) : this(time)
        {
            OnTrigger = onTrigger;
        }

        public event System.Action OnTrigger;

        private float time;
        private float currentTime;

        public bool Update(float dt)
        {
            currentTime -= dt;

            if(currentTime < 0.0f)
            {
                currentTime = time;
                OnTrigger?.Invoke();
                return true;
            }
            return false;
        }

        public void Reset(float newTime)
        {
            currentTime = newTime;
            time = newTime;
        }

        public void Reset()
        {
            Reset(time);
        }
    }

    public class Timer
    {
        public Timer(float time)
        {
            this.time = curTime = time;
        }

        public Timer(float time, System.Action onTrigger) : this(time)
        {
            OnTrigger = onTrigger;
        }

        public float percentage => Mathf.Max(0, curTime / time);

        public float currentTime => curTime;

        public event System.Action OnTrigger;

        private bool finished;
        private float time;
        private float curTime;

        public bool Update(float dt)
        {
            if(finished)
                return false;

            if(curTime < 0.0f)
            {
                finished = true;
                OnTrigger?.Invoke();

                return true;
            }

            curTime -= dt;

            return false;
        }

        public void Reset(float newTime)
        {
            curTime = newTime;
            time = newTime;
            finished = false;
        }

        public void Reset()
        {
            Reset(time);
        }

        public void Invalidate()
        {
            curTime = 0f;
            finished = true;
        }
    }

    public class PingPongTimer
    {
        public PingPongTimer(float newTime)
        {
            time = newTime;
            currentTime = newTime;
        }

//        public PingPongTimer(float time, System.Action onTrigger) : this(time)
//        {
//            OnTrigger = onTrigger;
//        }

//        public event System.Action OnTrigger;

        private float time;
        private float currentTime;
        private bool flipped;

        public float Percentage => flipped
            ? 1 - Mathf.Max(0, currentTime / time)
            : Mathf.Max(0, currentTime / time);

        public bool Update(float dt)
        {
            currentTime -= dt;

            if (currentTime < 0.0f)
            {
                currentTime = time;
                flipped = !flipped;
//                OnTrigger?.Invoke();
                return true;
            }

            return false;
        }

        public void Reset(float newTime)
        {
            currentTime = newTime;
            time = newTime;
            flipped = false;
        }

        public void Reset()
        {
            Reset(time);
        }
        
        /*
        [Header("General")]
        public float overallTime;
        public AnimationCurve multiplier = AnimationCurve.Constant(0f, 1f, .5f);
		
        private float Percentage => currentTime / overallTime;
        private float ZoneMultiplier => multiplier.Evaluate(Percentage);
        private float currentTime;
		
        public void UpdatePendulum(float dt)
        {
            // Advance the current time
            currentTime = Mathf.Repeat(currentTime + dt, overallTime);
        }
        
        public static float operator *(float f, Pendulum p) => f * p.ZoneMultiplier;
        */
    }
}