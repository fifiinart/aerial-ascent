using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private Grappling grappleController;
    public Transform groundCheckCollider;

    [Header("Movement")]
    public float speed = 10f;
    public float defaultGravity = 3f;
    [Range(0, 1)]
    public float horizontalDamping = .5f;
    [Range(0, 1)]
    public float stoppedDamping = .5f;
    [Range(0, 1)]
    public float turningDamping = .5f;

    [Header("Jump")]
    public float jumpForce = 10f;
    [Range(0, 1)]
    public float cutJumpHeight = .5f;
    public bool onGround = false;

    [Header("Jump Leniency")]
    public float jumpBufferTime = .25f;
    private float jumpBufferTimer = 0f;
    public float coyoteTime = .25f;
    private float coyoteTimer = 0f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grappleController = GetComponent<Grappling>();
    }

    private float CalculateXVel(float inputX)
    {
        float momentum = rb.velocity.x;
        momentum += inputX;
        momentum *= Mathf.Pow(1f - GetCorrectDamping(inputX, momentum), Time.deltaTime * 10f);
        return momentum;
    }

    private float GetCorrectDamping(float inputX, float momentum)
    {
        if (Mathf.Abs(inputX) < .01) return stoppedDamping;
        else if (Mathf.Sign(inputX) != Mathf.Sign(momentum)) return turningDamping;
        return horizontalDamping;
    }

    private void RotatePlayer(float x)
    {
        if (x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (x > 0) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }


    // Update is called once per frame
    void Update()
    {
        coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;

        if (onGround) coyoteTimer = coyoteTime;
        if (Input.GetButtonDown("Jump")) jumpBufferTimer = jumpBufferTime;

        float xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(CalculateXVel(xInput), CalculateYVel());

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));
        anim.SetFloat("SpeedY", rb.velocity.y);
        anim.SetBool("InAir", !onGround);
        RotatePlayer(xInput);
    }

    private float CalculateYVel()
    {
        float velocity = rb.velocity.y;
        if ((jumpBufferTimer > 0) && (coyoteTimer > 0))
        {
            jumpBufferTimer = 0;
            coyoteTimer = 0;
            velocity = jumpForce;
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (rb.velocity.y > 0)
            {
               velocity *= cutJumpHeight;
            }
        }
        return velocity;
    }

    private void FixedUpdate()
    {
        UpdateGrounded();
        //rb.gravityScale = defaultGravity;
        //if (rb.velocity.y < 0)
        //{
        //    rb.gravityScale *= fallGravityMultiplier;
        //}
        //else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        //{
        //    rb.gravityScale *= variableJumpGravityMultiplier;
        //}
    }

    private void UpdateGrounded()
    {
        onGround = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundMask);
        if (colliders.Length > 0)
        {
            onGround = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Tile"))
        {
            onGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            onGround = false;
        }
    }
}
