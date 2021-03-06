﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attractor : MonoBehaviour
{
    Rigidbody body;
    public float G = 0.1f;
    public float nearMass {
        get {
            Vector3 scale = transform.lossyScale;
            return body.mass * scale.x * scale.y * scale.z;
        }
    }

    void Start() {
        body = GetComponent<Rigidbody>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
    }

    void FixedUpdate() {
        foreach (Attracted obj in Attracted.ones) {
            Vector3 down = (transform.position - obj.transform.position);
            Vector3 gravity = down.normalized * G * nearMass * obj.body.mass / down.magnitude;
            obj.body.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
