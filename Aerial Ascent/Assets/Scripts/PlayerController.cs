using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Ground Collision")]
    public LayerMask groundMask;
    public Transform groundCheckCollider;
    public float groundCheckRadius;

    [Header("Jump")]
    public float jumpForce = 5;

    private float bufferJumpTimer = 0;
    public float bufferJumpTime = 0.2f;

    private float coyoteJumpTimer = 0;
    public float coyoteJumpTime = 0.25f;

    public bool onGround;

    [Range(0, 1)]
    public float cutJumpHeight = 0.5f;

    [Header("Movement")]
    public float speed = 1;

    // How much momentum should be preserved within 10 seconds
    [Range(0, 1)]
    public float horizontalDamping = 0.5f;
    [Range(0, 1)]
    public float stoppedDamping = 0.5f;
    [Range(0, 1)]
    public float turningDamping = 0.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        onGround = CheckIfGrounded();

        UpdateCoyoteJumpTimer();

        UpdateBufferJumpTimer();

        HandleCutJumps();

        Jump();

        float velocity = CalculateDampedVelocity();

        rb.velocity = new Vector2(velocity, rb.velocity.y);
    }

    private float CalculateDampedVelocity()
    {
        float velocity = rb.velocity.x;
        velocity += Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            velocity *= Mathf.Pow(1f - stoppedDamping, Time.deltaTime * 10f);
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(velocity))
            velocity *= Mathf.Pow(1f - turningDamping, Time.deltaTime * 10f);
        else
            velocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);
        return velocity;
    }

    private void Jump()
    {
        if ((bufferJumpTimer > 0) && (coyoteJumpTimer > 0))
        {
            bufferJumpTimer = 0;
            coyoteJumpTimer = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void HandleCutJumps()
    {
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpHeight);
            }
        }
    }

    private void UpdateBufferJumpTimer()
    {
        bufferJumpTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            bufferJumpTimer = bufferJumpTime;
        }
    }

    private void UpdateCoyoteJumpTimer()
    {
        coyoteJumpTimer -= Time.deltaTime;
        if (onGround)
        {
            coyoteJumpTimer = coyoteJumpTime;
        }
    }

    private bool CheckIfGrounded()
    {
        //Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        //Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        //bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, groundMask);
        //return bGrounded;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundMask);
        return colliders.Length > 0;
    }
}
