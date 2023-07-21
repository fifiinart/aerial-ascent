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
    public ParticleSystem dust;

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
    public LayerMask canGrappleMask;
    public float speed = 1;
    private bool facingRight = true;

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
    public AudioClip bounceSound;

    [Header("Game Over")]
    public bool gameOver = false;
    public Vector2 spawnPosition = Vector2.zero;
    public float timeBeforeRespawn = 0.5f;
    private float respawnTimer = 0;
    public AudioClip deathSound;
    private bool inControl = false;
    public AudioSource aud;

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
        if (!gameOver && inControl)
        {
            HandleCutJumps();

            Jump();
            UpdateCoyoteJumpTimer();

            UpdateBufferJumpTimer();
        }
        else if (respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
        } else if (respawnTimer < 0)
        {
            RespawnPlayer();
        }


        if (transform.position.y < -20)
        {
            KillPlayer();
        }
    }

    private void RespawnPlayer()
    {
        gameOver = false;
        transform.position = spawnPosition;
        anim.SetBool("Dead", false);
    }

    void CreateDust()
    {
        dust.Play();
    }

    public void PlayerInControl()
    {
        Time.timeScale = 1;
        inControl = true;
    }

    public void PlayerWin()
    {
        //...
    }

    private void FlipPlayer()
    {
        if (grappling.isGrappling)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan2(grappling.directionToGrapplePos.y, grappling.directionToGrapplePos.x);

            if (angle > 90 || angle < -90)
            {
                sprite.rotation = Quaternion.Euler(0, 180, -(angle - 90) + 90);
            }
            else
            {
                sprite.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            sprite.rotation = Quaternion.Euler(0, facingRight ? 0f : 180f, 0);
        }
    }

    private float CalculateDampedVelocity()
    {
        float velocity = rb.velocity.x;
        velocity += inControl ? Input.GetAxisRaw("Horizontal") : 0f;
        if (Input.GetAxisRaw("Horizontal") > 0.05) facingRight = true;
        if (Input.GetAxisRaw("Horizontal") < -0.05) facingRight = false;

        if (grappling.isGrappling)
            velocity *= 1; // don't damp grapple
        else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            velocity *= Mathf.Pow(1f - stoppedDamping, Time.fixedDeltaTime * 10f);
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(velocity))
        {
            velocity *= Mathf.Pow(1f - turningDamping, Time.fixedDeltaTime * 10f);
        }
        else
        {
            velocity *= Mathf.Pow(1f - horizontalDamping, Time.fixedDeltaTime * 10f);
        }
        return velocity;
    }

    private void Jump()
    {
        if ((bufferJumpTimer > 0) && (coyoteJumpTimer > 0) && inControl)
        {
            bufferJumpTimer = 0;
            coyoteJumpTimer = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (grappling.isGrappling)
            {
                grappling.StopGrappling();
            }
            CreateDust();
        }
    }

    private void HandleCutJumps()
    {
        if (Input.GetButtonUp("Jump") && inControl)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpHeight);
        }
    }

    private void UpdateBufferJumpTimer()
    {
        bufferJumpTimer -= Time.fixedDeltaTime;
        if (Input.GetButtonDown("Jump") && inControl)
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
            aud.PlayOneShot(bounceSound, 1f);
        }
        else if (collision.gameObject.CompareTag("Deadly"))
        {
            KillPlayer();
        }
    }

    private void HandleBounce(Collision2D collision)
    {

        // check all circlecasts
        rb.velocity = Vector2.zero;
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, canGrappleMask);
        Collider2D[] ceilingColliders = Physics2D.OverlapCircleAll(ceilingCheckCollider.position, groundCheckRadius, canGrappleMask);
        Collider2D[] leftColliders = Physics2D.OverlapCircleAll(leftCheckCollider.position, groundCheckRadius, canGrappleMask);
        Collider2D[] rightColliders = Physics2D.OverlapCircleAll(rightCheckCollider.position, groundCheckRadius, canGrappleMask);

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

    [ContextMenu("Set Spawn to Current Position")]
    public void setSpawnToCurrentPosition() { spawnPosition = new Vector2(transform.position.x, transform.position.y);}

    private void KillPlayer()
    {
        if (!gameOver) 
        {
        respawnTimer = timeBeforeRespawn;
        aud.PlayOneShot(deathSound, 1f);
        gameOver = true;
        rb.velocity = new Vector2(0, 0);
        anim.SetBool("Dead", true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CheckPoint"))
        {
            Checkpoint checkpoint = collision.gameObject.GetComponent<Checkpoint>();
            spawnPosition = (Vector2) collision.transform.position;
            checkpoint.Activate();
        }
        else if (collision.gameObject.CompareTag("Ring"))
        {
            grappling.StopGrappling();
        }
    }

    
}
