﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnderGravity : MonoBehaviour
{
    public static List<UnderGravity> ones = new List<UnderGravity>();
    [HideInInspector]
    public Rigidbody body;

    void Start() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.sleepThreshold = 1;
    }

    void OnEnable() {
        ones.Add(this);
    }

    void OnDisable() {
        ones.Remove(this);
    }

    void FixedUpdate() {
    }
}
