using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MCMC
{
    public class Metropolis2d
    {
        public const int LIMIT_RESET_LOOP_COUNT = 100;
        public Texture2D ProbTex { get; private set; }
        public float Sigma { get; private set; }

        private Vector2 _curr;
        private float _currDensity = 0f;
        private Vector2 _stddevAspect;

        public Metropolis2d(Texture2D probTex, float sigma)
        {
            this.ProbTex = probTex;
            this.Sigma = sigma;
        }

        public void Reset()
        {
            for (var i = 0; _currDensity <= 0f && i < LIMIT_RESET_LOOP_COUNT; i++)
            {
                _curr = new Vector2(Random.value, Random.value);
                _currDensity = Density(_curr);
            }
        }

        public IEnumerable<Vector2> Chain(int nInitialize, int limit, float threshold)
        {
            Reset();

            for (var i = 0; i < nInitialize; i++)
                Next(threshold);

            for (var i = 0; i < limit; i++)
            {
                yield return _curr;
                Next(threshold);
            }
        }

        void Next(float threshold)
        {
            var next = Sigma * BoxMuller.Gaussian() + _curr;
            next.x -= Mathf.Floor(next.x);
            next.y -= Mathf.Floor(next.y);

            var densityNext = Density(next);
            bool flag1 = _currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= Random.value;
            bool flag2 = densityNext > threshold;
            if (flag1 && flag2)
            {
                _curr = next;
                _currDensity = densityNext;
            }
        }

        float Density(Vector2 curr)
        {
            return ProbTex.GetPixelBilinear(curr.x, curr.y).r;
        }

    }
}
