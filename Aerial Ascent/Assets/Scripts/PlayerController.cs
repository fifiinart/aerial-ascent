using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Grappling grappling;

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

    [Header("Bounce")]
    public float minHorizontalBounce = 40f;
    public float minVerticalBounce = 30f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grappling = GetComponent<Grappling>();
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

        FlipPlayer();
        anim.SetFloat("SpeedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("SpeedY", rb.velocity.y);
        anim.SetBool("InAir", !onGround);
        anim.SetBool("Grappling", grappling.isGrappling);
    }

    private void FlipPlayer()
    {
        if (grappling.isGrappling)
        {
            float y = grappling.directionToGrapplePos.x < 0 ? 180f : 0f;
            float z = grappling.grappleAngle;

            transform.rotation = Quaternion.Euler(0, y, z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, (float)(Input.GetAxisRaw("Horizontal") < 0.01f ? 180f : 0f), 0);
        }
    }

    private float CalculateDampedVelocity()
    {
        float velocity = rb.velocity.x;
        velocity += Input.GetAxisRaw("Horizontal");

        if (grappling.isGrappling)
            velocity *= 1; // don't damp grapple
        else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
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
            if (grappling.isGrappling)
            {
                grappling.StopGrappling();
            }
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
        if (CanJump())
        {
            coyoteJumpTimer = coyoteJumpTime;
        }
    }

    private bool CanJump()
    {
        return onGround || grappling.isGrappling;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (grappling.isGrappling)
            grappling.StopGrappling();
        if (collision.gameObject.CompareTag("Bouncy"))
        {
            HandleBounce(collision);
        }
    }

    private void HandleBounce(Collision2D collision)
    {
        var contact = collision.GetContact(0);
        var difference = (Vector2)transform.position - contact.point;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            if (contact.relativeVelocity.x > 0)
                // collide with left/right wall
                rb.velocity = new Vector2(
                    Mathf.Max(minHorizontalBounce, contact.relativeVelocity.x),
                    Mathf.Abs(contact.relativeVelocity.y)
                );
            else
                rb.velocity = new Vector2(
                    Mathf.Min(-minHorizontalBounce, contact.relativeVelocity.x),
                    Mathf.Abs(contact.relativeVelocity.y)
                );
        }
        else
        {
            if (contact.relativeVelocity.y > 0)
                rb.velocity = new Vector2(
                    -contact.relativeVelocity.x,
                    Mathf.Max(minVerticalBounce, contact.relativeVelocity.y)
                );
            else
                rb.velocity = new Vector2(
                    -contact.relativeVelocity.x,
                    Mathf.Min(-minVerticalBounce, contact.relativeVelocity.y)
                );
        }
    }
}
