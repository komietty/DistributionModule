using UnityEngine;
using System.Collections.Generic;
using System;

namespace MCMC
{
    public class Metropolis3d
    {
        public static readonly int limitResetLoopCount = 100;
        public static readonly int weightReferenceloopCount = 500;
        public Vector3 Scale { get; private set; }
        public Vector4[] Data { get; private set; }
        public Func<float, float, float, float> DensityFunc{ get; private set; }

        Vector3 _curr;
        float _currDensity = 0f;

        public Metropolis3d(Vector3 scale, Vector4[] data = null, Func<float, float, float, float> densityFunc = null)
        {
            this.Scale = scale;
            this.Data = data;
            this.DensityFunc = densityFunc;
        }

        public void Reset()
        {
            for (var i = 0; _currDensity <= 0f && i < limitResetLoopCount; i++)
            {
                _curr = Vector3.Scale(new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value), Scale);
                _currDensity = Density(_curr);
            }
        }

		public IEnumerable<Vector3> Chain(int nInitialize, int limit, float threshold)
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
            Vector3 next = GaussianDistribution3d.GenerateRandomPointStandard() + _curr;

            var densityNext = Density(next);
            bool flag1 = _currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= UnityEngine.Random.value;
            bool flag2 = densityNext > threshold;
            if (flag1 && flag2)
            {
                _curr = next;
                _currDensity = densityNext;
            }
        }

        float Density(Vector3 pos)
        {
            float weight = 0f;

            if(this.DensityFunc != null)
            {
                for (int i = 0; i < weightReferenceloopCount; i++)
                {
                    var x = UnityEngine.Random.value * Scale.x;
                    var y = UnityEngine.Random.value * Scale.y;
                    var z = UnityEngine.Random.value * Scale.z;
                    var posi = new Vector3(x, y, z);
                    float mag = Vector3.SqrMagnitude(pos - posi);
                    weight += Mathf.Exp(-mag) * DensityFunc(x, y, z);
                }
            }
            else if (this.Data != null)
            {
                for (int i = 0; i < weightReferenceloopCount; i++)
                {
                    int id = (int)Mathf.Floor(UnityEngine.Random.value * (Data.Length - 1));
                    Vector3 posi = Data[id];
                    float mag = Vector3.SqrMagnitude(pos - posi);
                    weight += Mathf.Exp(-mag) * Data[id].w;
                }
            }
            return weight;
        }
    }
}