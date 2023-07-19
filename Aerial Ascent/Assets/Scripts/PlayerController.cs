using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.TerrainTools;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    public Transform sprite;
    public Animator anim;
    private Grappling grappling;

    [Header("Ground Collision")]
    public LayerMask groundMask;
    public Transform groundCheckCollider;
    public Transform ceilingCheckCollider;
    public Transform leftCheckCollider;
    public Transform rightCheckCollider;
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

    [Header("Game Over")]
    public bool gameOver = false;
    public Vector2 spawnPosition = Vector2.zero;
    public float timeBeforeRespawn = 0.5f;
    private float respawnTimer = 0;


    void Start()
    {
        transform.position = spawnPosition;
        rb = GetComponent<Rigidbody2D>();
        grappling = GetComponent<Grappling>();
    }

    void FixedUpdate()
    {
        if (!gameOver)
        {

            CheckCollisions();

            float velocity = CalculateDampedVelocity();

            rb.velocity = new Vector2(velocity, rb.velocity.y);

            FlipPlayer();
            anim.SetFloat("SpeedX", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("SpeedY", rb.velocity.y);
            anim.SetBool("InAir", !onGround);
            anim.SetBool("Grappling", grappling.isGrappling);
        }
    }
    void Update()
    {
        if (!gameOver)
        {
            HandleCutJumps();

            Jump();
            UpdateCoyoteJumpTimer();

            UpdateBufferJumpTimer();
        }
        else if (timeBeforeRespawn > 0)
        {
            timeBeforeRespawn -= Time.deltaTime;
        } else if (timeBeforeRespawn < 0)
        {
            gameOver = false;
            transform.position = spawnPosition;
        }
    }

    private void FlipPlayer()
    {
        if (grappling.isGrappling)
        {
            float y = grappling.directionToGrapplePos.x < 0 ? 180f : 0f;
            float z = grappling.grappleAngle;

            sprite.rotation = Quaternion.Euler(0, y, z);
        }
        else
        {
            sprite.rotation = Quaternion.Euler(0, (float)(Input.GetAxisRaw("Horizontal") < 0.01f ? 180f : 0f), 0);
        }
    }

    private float CalculateDampedVelocity()
    {
        float velocity = rb.velocity.x;
        velocity += Input.GetAxisRaw("Horizontal");

        if (grappling.isGrappling)
            velocity *= 1; // don't damp grapple
        else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            velocity *= Mathf.Pow(1f - stoppedDamping, Time.fixedDeltaTime * 10f);
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(velocity))
            velocity *= Mathf.Pow(1f - turningDamping, Time.fixedDeltaTime * 10f);
        else
            velocity *= Mathf.Pow(1f - horizontalDamping, Time.fixedDeltaTime * 10f);
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
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpHeight);
        }
    }

    private void UpdateBufferJumpTimer()
    {
        bufferJumpTimer -= Time.fixedDeltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            bufferJumpTimer = bufferJumpTime;
        }
    }

    private void UpdateCoyoteJumpTimer()
    {
        coyoteJumpTimer -= Time.fixedDeltaTime;
        if (CanJump())
        {
            coyoteJumpTimer = coyoteJumpTime;
        }
    }

    private bool CanJump()
    {
        return onGround || grappling.isGrappling;
    }

    private void CheckCollisions()
    {
        //Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        //Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        //bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, groundMask);
        //return bGrounded;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundMask);
        onGround = colliders.Length > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (grappling.isGrappling)
            grappling.StopGrappling();
        if (collision.gameObject.CompareTag("Bouncy"))
        {
            HandleBounce(collision);
        }
        else if (collision.gameObject.CompareTag("Deadly"))
        {
            KillPlayer(collision);
        }
    }

    private void HandleBounce(Collision2D collision)
    {

        // check all circlecasts
        rb.velocity = Vector2.zero;
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundMask);
        Collider2D[] ceilingColliders = Physics2D.OverlapCircleAll(ceilingCheckCollider.position, groundCheckRadius, groundMask);
        Collider2D[] leftColliders = Physics2D.OverlapCircleAll(leftCheckCollider.position, groundCheckRadius, groundMask);
        Collider2D[] rightColliders = Physics2D.OverlapCircleAll(rightCheckCollider.position, groundCheckRadius, groundMask);

        if (rightColliders.Length > 0)
        {
            rb.velocity += new Vector2(
                Mathf.Min(-minHorizontalBounce, collision.relativeVelocity.x),
                Mathf.Abs(collision.relativeVelocity.y)
            );
        }
        else if (leftColliders.Length > 0)
        {
            rb.velocity += new Vector2(
                Mathf.Max(minHorizontalBounce, collision.relativeVelocity.x),
                Mathf.Abs(collision.relativeVelocity.y)
            );
        }
        else if (groundColliders.Length > 0)
        {
            rb.velocity += new Vector2(
                -collision.relativeVelocity.x,
                Mathf.Max(minVerticalBounce, collision.relativeVelocity.y)
            );
        }
        else if (ceilingColliders.Length > 0)
        {
            rb.velocity += new Vector2(
                -collision.relativeVelocity.x,
                Mathf.Min(-minVerticalBounce, collision.relativeVelocity.y)
            );
        }

    }

    private void KillPlayer(Collision2D collision)
    {
        gameOver = true;
        rb.velocity = new Vector2(0, 0);
        respawnTimer = timeBeforeRespawn;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CheckPoint"))
        {
            Checkpoint checkpoint = collision.gameObject.GetComponent<Checkpoint>();
            spawnPosition = (Vector2) collision.transform.position;
            checkpoint.used = true;
        }
    }
}
