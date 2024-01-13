using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class TickUpdaterBase : MonoBehaviour, ITickUpdater
    {
        public event Action<long> onTick;

        public long tick { get; private set; }
        public double interval { get; private set; }
        public double time => tick * interval;
        public double elapsedTime { get; protected set; }

        public long processibleTick
        {
            get
            {
                var processibleTick = (long)(elapsedTime / interval);
                return processibleTick;
            }
        }

        public bool initialized { get; protected set; }

        public void Initialize(long tick = 0, double interval = 1 / 60.0, double elapsedTime = 0)
        {
            this.tick = tick;
            this.interval = interval;
            this.elapsedTime = elapsedTime;

            initialized = true;
        }

        public void Deinitialize()
        {
            StopCoroutine("TickUpdateLoop");

            onTick = null;
            tick = 0;
            interval = 1 / 60.0;
            elapsedTime = 0;

            initialized = false;
        }

        public void Run()
        {
            Run(tick, interval, elapsedTime);
        }

        public void Run(long tick, double interval, double elapsedTime)
        {
            this.tick = tick;
            this.interval = interval;
            this.elapsedTime = elapsedTime;

            StopCoroutine("TickUpdateLoop");
            StartCoroutine("TickUpdateLoop");
        }

        private IEnumerator TickUpdateLoop()
        {
            while (true)
            {
                while (tick <= processibleTick)
                {
                    TickBody();
                }

                yield return null;

                OnElapsedTimeUpdate();
            }
        }

        private void TickBody()
        {
            onTick?.Invoke(tick++);
        }

        protected virtual void OnElapsedTimeUpdate()
        {
            elapsedTime += UnityEngine.Time.deltaTime;
        }
    }
}
