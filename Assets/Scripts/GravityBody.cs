using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
	GravityAttractor attractor;
	Rigidbody rb;

	void Awake()
	{
		attractor = GameObject.FindGameObjectWithTag("Attractor").GetComponentInChildren<GravityAttractor>();
		rb = GetComponent<Rigidbody>();

		// Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.useGravity = false;
	}

	void FixedUpdate()
	{
		// Allow this body to be influenced by planet's gravity
		attractor.Attract(rb);
	}
}
