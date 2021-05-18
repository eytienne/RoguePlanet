using UnityEngine;

public class InverseTModifier : BezierCurve.TModifier
{
    public override float Modify(float x) {
        return 1 - x;
    }
}