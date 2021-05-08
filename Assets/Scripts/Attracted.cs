using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attracted : MonoBehaviour
{
    public static List<Attracted> ones = new List<Attracted>();
    [HideInInspector]
    public Rigidbody body;

    void Start() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
    }

    void OnEnable() {
        ones.Add(this);
    }

    void OnDisable() {
        ones.Remove(this);
    }

}
