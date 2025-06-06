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
        public double elapsedTime { get; protected set; }

        public long processibleTick
        {
            get
            {
                var processibleTick = (long)(elapsedTime / interval);
                return processibleTick;
            }
        }

        public void Run(long tick, double interval, double elapsedTime)
        {
            this.tick = tick;
            this.interval = interval;
            this.elapsedTime = elapsedTime;

            StopCoroutine("TickUpdateLoop");
            StartCoroutine("TickUpdateLoop");
        }

        public void Stop()
        {
            StopCoroutine("TickUpdateLoop");
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
            onTick?.Invoke(tick);
            tick++;
        }

        protected virtual void OnElapsedTimeUpdate()
        {
            elapsedTime += UnityEngine.Time.smoothDeltaTime;
        }
    }
}
