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
    public float horizontalDamping;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float fallGravityMultiplier = 2.5f;
    public float variableJumpGravityMultiplier = 2f;
    public bool onGround = false;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grappleController = GetComponent<Grappling>();
    }

    private void CalculateXVel(float inputX)
    {
        float momentum = rb.velocity.x;
        momentum += inputX;
        momentum *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);
        rb.velocity = new Vector2(momentum, rb.velocity.y);
    }   

    private void RotatePlayer(float x)
    {
        if (x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (x > 0) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        float xInput = Input.GetAxisRaw("Horizontal");
        CalculateXVel(xInput);

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));
        anim.SetFloat("SpeedY", rb.velocity.y);
        anim.SetBool("InAir", !onGround);
        RotatePlayer(xInput);
    }

    private void FixedUpdate()
    {
        isGrounded();
        rb.gravityScale = defaultGravity;
        if (grappleController.isGrappling)
        {
            rb.gravityScale = 0;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale *= fallGravityMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale *= variableJumpGravityMultiplier;
        }
    }

    private void isGrounded()
    {
        onGround = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, mask);
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
