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

    [Header("Jump")]
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Walk(float speed)
    {
        rb.velocity = new Vector2(speed * speed, rb.velocity.y);

        anim.SetFloat("SpeedX", Mathf.Abs(speed));
        RotatePlayer(speed);
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
            rb.velocity += Vector2.up * jumpForce;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        float xInput = Input.GetAxis("Horizontal");

        Walk(xInput);

        /*
        if(Input.GetKeyDown(KeyCode.W))
        {
           rb.velocity = new Vector2(rb.velocity.x, 0);
           rb.velocity += Vector2.up * jumpForce;
        }
        */

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
