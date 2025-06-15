using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollisionHandler : MonoBehaviour
{
	[SerializeField] private float minCollisionForce = 2f;
	[SerializeField] private float dampingFactor = 0.6f;
	[SerializeField] private float maxSwingSpeed = 15f;

	private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {    	
        LimitVelocity();
    }


    private void LimitVelocity()
    {
    	if (rb.velocity.magnitude > maxSwingSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSwingSpeed;            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;
        if (collisionForce > minCollisionForce)
        {
        	rb.velocity *= dampingFactor;
        	rb.angularVelocity *= dampingFactor;
        }
    }
}
