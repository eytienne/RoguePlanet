using System;
using System.Linq;
using UnityEngine;

namespace CMath
{
    public static class Tooling
    {
        static Tooling() {
            Debug.Assert(GetBinCoeff(5, 3) == 10);
        }

        public static long GetBinCoeff(long N, long K) {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            long ret = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++) {
                ret *= N--;
                ret /= d;
            }
            return ret;
        }

        public static float GetBernsteinPolynomial(float u, long m, long i) {
            return GetBinCoeff(m, i) * Mathf.Pow(u, i) * Mathf.Pow(1 - u, m - i);
        }

        public static Vector3 GetBezierCurvePoint(float t, params Vector3[] controlPoints) {
            Vector3 ret = Vector3.zero;
            int n = controlPoints.Length - 1;
            for (int i = 0; i <= n; i++) {
                ret += GetBernsteinPolynomial(t, n, i) * controlPoints[i];
            }
            return ret;
        }

        public static Vector3 GetBezierCubicPoint(cfloat t, params Vector3[] controlPoints) {
            int len = controlPoints.Length;
            if (len != 4) throw new ArgumentException("Should have length of 4 but has " + len, "controlPoints");
            return GetBezierCubicPoint(t, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
        }

        public static Vector3 GetBezierCubicPoint(cfloat t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            return ((1 - t) ^ 3) * p0 + 3 * (t ^ 1) * ((1 - t) ^ 2) * p1 + 3 * (t ^ 2) * ((1 - t) ^ 1) * p2 + (t ^ 3) * p3;
        }

        public static Func<float, Vector3> SinusoidalFromBezierCubic01(Vector3[] curve) {
            return (float t) => {
                t *= 2;
                if (t > 1) t = 1 - (t - 1);
                Vector3 point = Tooling.GetBezierCubicPoint(t, curve);
                point.y = 2 * point.y - 1;
                return point;
            };
        }
    }

    public struct cfloat
    {
        public float value;
        public cfloat(float f) {
            value = f;
        }

        public static implicit operator float(cfloat cf) => cf.value;
        public static implicit operator cfloat(float f) => new cfloat(f);
        public static float operator ^(cfloat a, int k) => Mathf.Pow(a, k);
        public static cfloat operator -(float a, cfloat b) => new cfloat(a - b.value);
        public static cfloat operator -(cfloat a, float b) => new cfloat(a.value - b);
    }

}