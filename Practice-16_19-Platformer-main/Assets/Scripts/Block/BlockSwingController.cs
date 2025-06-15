using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PlayerBlockDropper))]
public class BlockSwingController : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float swingForceMultiplier = 2f;
    [SerializeField] private float maxSwingAngle = 45f;
    [SerializeField] private float maxSwingSpeed = 15f;
    [SerializeField] private float rotationDamping = 0.9f;

    private DynamicRopeSystem ropeSystem;
    public bool isSwingActive;
    private Vector2 previousTouchPosition;

    private void Awake()
    {
        ropeSystem = GetComponent<DynamicRopeSystem>();
    }

    private void FixedUpdate()
    {
        if (!isSwingActive) // || ropeSystem.currentBlockRb == null
        {
        	return;
        } 
        HandleSwingInput();
    }

    private void HandleSwingInput()
    {
        if (Input.touchCount == 0)
        {
        	return;
        }
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                previousTouchPosition = touch.position;
                break;
            case TouchPhase.Moved:
            	Vector2 swipeDelta = touch.position - previousTouchPosition;
            	ApplySwing(swipeDelta);
            	previousTouchPosition = touch.position;
            	break;
        }

        LimitSwingAngle();
        LimitVelocity();
    }

    private void LimitVelocity()
    {
        if (ropeSystem.currentBlockRb.velocity.magnitude > maxSwingSpeed)
        {
            ropeSystem.currentBlockRb.velocity = ropeSystem.currentBlockRb.velocity.normalized * maxSwingSpeed;            
        }
    }

    private void ApplySwing(Vector2 swipeDelta)
    {
    	if(swipeDelta.magnitude < 5f) return;
        Vector2 force = new Vector2(
            swipeDelta.x * swingForceMultiplier,
            Mathf.Abs(swipeDelta.y) * 0.5f * swingForceMultiplier
        );

        ropeSystem.currentBlockRb.AddForce(force, ForceMode2D.Impulse);
    }

    private void LimitSwingAngle()
    {
        float currentAngle = Vector2.Angle(Vector2.down, ropeSystem.currentBlockRb.transform.up);
        if (currentAngle > maxSwingAngle)
        {
            ropeSystem.currentBlockRb.angularVelocity *= rotationDamping;
            float correctionForce = Mathf.Clamp01((currentAngle - maxSwingAngle) / 10f);
            ropeSystem.currentBlockRb.AddTorque(-ropeSystem.currentBlockRb.angularVelocity * correctionForce);
        }
    }

    public void EnableSwing(bool enable)
    {
        isSwingActive = enable;
        if (enable && ropeSystem.currentBlockRb != null)
        {
        	ropeSystem.currentBlockRb.drag = 0.5f;
        	ropeSystem.currentBlockRb.angularDrag = 0.5f;
        }
    }
}