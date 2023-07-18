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

    [Header("Movement")]
    public float speed = 10f;
    public float defaultGravity = 3f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float fallGravityMultiplier = 2.5f;
    public float variableJumpGravityMultiplier = 2f;
    public bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Walk(float inputX)
    {
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
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

        float xInput = Input.GetAxis("Horizontal");
        Walk(xInput);

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));
        anim.SetFloat("SpeedY", rb.velocity.y);
        anim.SetBool("InAir", !onGround);
        RotatePlayer(xInput);
    }

    private void FixedUpdate()
    {
        rb.gravityScale = defaultGravity;
        if (rb.velocity.y < 0)
        {
            rb.gravityScale *= fallGravityMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale *= variableJumpGravityMultiplier;
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
