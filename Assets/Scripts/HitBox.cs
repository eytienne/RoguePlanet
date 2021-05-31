using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public event Action hit;

    public void Hit() {
        hit?.Invoke();
    }
}