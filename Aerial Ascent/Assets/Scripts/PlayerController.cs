using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 10f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Walk(Vector2 dir)
    {
        rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y));

    }


    // Update is called once per frame
    void Update()
    {

        if(Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier -1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        /*
        bool jumpInput = Input.GetButtonDown("Jump");
        bool jumpInputReleased = Input.GetButtonUp("Jump");

        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

        //need to add on ground conditions
        if(jumpInput)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(jumpInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y-gravityChange - Time.deltaTime);
        }

        if(xInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(xInput), 1, 1);
        }
        */

        Vector2 dir = new Vector2(xInput, yInput);

        Walk(dir);

        /*
        if(Input.GetKeyDown(KeyCode.W))
        {
           rb.velocity = new Vector2(rb.velocity.x, 0);
           rb.velocity += Vector2.up * jumpForce;
        }
        */
        
    }
}
