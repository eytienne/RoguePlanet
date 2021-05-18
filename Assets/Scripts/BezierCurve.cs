using CMath;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{

    public int nSegments;
    public TModifier modifierX;
    public TModifier modifierY;
    public float scale = 10f;
    public Vector2[] controlPoints;

    LineRenderer lineRenderer;

    public abstract class TModifier : MonoBehaviour
    {
        public abstract float Modify(float t);
    }

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
        lineRenderer.positionCount = nSegments + 1;
        for (int i = 0; i < nSegments + 1; i++) {
            var t = (float)i / nSegments;
            Vector3 point = Tooling.GetBezierCurvePoint(t, finalControlPoints);
            if (modifierX != null) point.x = modifierX.Modify(point.x);
            if (modifierY != null) point.y = modifierY.Modify(point.y);
            var worldPoint = transform.TransformPoint(scale * point);
            _OnDrawGizmos += () => {
                Gizmos.color = Color.Lerp(Color.yellow, Color.red, modifierX != null ? modifierX.Modify(t) : t);
                Gizmos.DrawSphere(worldPoint, 0.5f);
            };
            lineRenderer.SetPosition(i, worldPoint);
        }
    }
}
