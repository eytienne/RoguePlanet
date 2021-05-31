using CMath;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Sinusoidal : MonoBehaviour
{

    public int nSegments = 10;
    public float scale = 10f;
    public Vector2[] controlPoints;

    LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        Initialize();
    }

    void OnValidate() {
        if (lineRenderer == null) return;
        Initialize();
    }

    event Action _OnDrawGizmos;

    void OnDrawGizmos() {
        _OnDrawGizmos?.Invoke();
    }

    void Initialize() {
        _OnDrawGizmos = null;
        if (controlPoints.Length != 4) throw new UnityException("Should give 4 control points");

        Vector3[] finalControlPoints = controlPoints.Select(v => {
            Vector3 ret = v;
            ret.z = transform.position.z;
            return ret;
        }).ToArray();
        _OnDrawGizmos += () => {
            Gizmos.color = Color.cyan;
            foreach (var controlPoint in finalControlPoints) {
                Gizmos.DrawSphere(transform.TransformPoint(scale * controlPoint), 0.6f);
            }
        };

        var func = Tooling.SinusoidalFromBezierCubic01(finalControlPoints);

        lineRenderer.positionCount = nSegments * 2 + 1;
        for (int i = 0; i < nSegments * 2 + 1; i++) {
            float t = (float)i / (nSegments * 2);
            Vector3 point = func(t);
            if (t > 0.5) point.x = 2 - point.x;
            Debug.Log($"i {i} t {t} point {point}");

            Vector3 worldPoint = transform.TransformPoint(scale * point);

            _OnDrawGizmos += () => {
                Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
                Gizmos.DrawSphere(worldPoint, 0.5f);
            };
            lineRenderer.SetPosition(i, worldPoint);
        }
    }
}
