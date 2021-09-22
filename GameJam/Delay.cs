using System;
using System.Collections.Generic;

namespace GameJam
{
    class Delay
    {
        public static List<Delay> delays = new List<Delay>();

        private float clock = 0;
        private readonly float time;
        private readonly Action callback;

        public Delay(float _time, Action _callback)
        {
            time = _time;
            callback = _callback;

            delays.Add(this);
        }

        public void Update(float deltaTime)
        {
            clock += deltaTime;

            if (clock >= time)
            {
                callback();
                delays.Remove(this);
            }
        }
    }
}
