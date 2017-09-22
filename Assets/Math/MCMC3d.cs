using UnityEngine;
using System.Collections.Generic;

namespace komietty.Math
{
    public class MCMC3d
    {
        public static readonly int limitResetLoopCount = 100;
        public static readonly int weightReferenceloopCount = 500;
        public Vector4[] Data { get; private set; }
        public Vector3 Scale { get; private set; }

        Vector3 _curr;
        float _currDensity = 0f;

        public MCMC3d(Vector4[] data, Vector3 scale)
        {
            this.Data = data;
            this.Scale = scale;
        }

        public void Reset()
        {
            for (var i = 0; _currDensity <= 0f && i < limitResetLoopCount; i++)
            {
                _curr = new Vector3(Scale.x * Random.value, Scale.y * Random.value, Scale.z * Random.value);
                _currDensity = Density(_curr);
            }
        }

        public IEnumerable<Vector3> Sequence(int nInitialize, int limit)
        {
            return Sequence(nInitialize, limit, 0);
        }

        public IEnumerable<Vector3> Sequence(int nInitialize, int limit, int nSkip)
        {
            Reset();

            for (var i = 0; i < nInitialize; i++)
                Next();

            for (var i = 0; i < limit; i++)
            {
                for (var j = 0; j < nSkip; j++)
                    Next();
                yield return _curr;
                Next();
            }
        }

        void Next()
        {
            Vector3 next = GaussianDistributionCubic.GenerateRandomPointStandard() + _curr;

            var densityNext = Density(next);
            if (_currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= Random.value)
            {
                _curr = next;
                _currDensity = densityNext;
            }
        }

        float Density(Vector3 pos)
        {
            float weight = 0f;
            for (int i = 0; i < weightReferenceloopCount; i++)
            {
                int id = (int)Mathf.Floor(Random.value * (Data.Length - 1));
                Vector3 posi = Data[id];
                float mag = Vector3.SqrMagnitude(pos - posi);
                weight += Mathf.Exp(-mag) * Data[id].w;
            }
            return weight;
        }
    }
}