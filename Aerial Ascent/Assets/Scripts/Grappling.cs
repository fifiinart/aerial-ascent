using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{

    public float speed = 20f;
    public float distance = 10f;
    RaycastHit2D hit;
    public LayerMask mask;
    public LineRenderer lineRenderer;
    private Vector2 grapplingPos;
    public bool isGrappling = false;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();   
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //get mouse pos
        //get look direction

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (mousePos - (Vector2)transform.position).normalized;

        Debug.DrawRay(transform.position, lookDirection);
        
        if (Input.GetMouseButtonDown(0) && isGrappling == false)
        {
            hit = Physics2D.Raycast(transform.position, lookDirection, distance, mask);
            if (hit)
            {
                grapplingPos = hit.point;
                isGrappling = true;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            //stop Grappling
            isGrappling = false;
        }
        if (lineRenderer.enabled = isGrappling) // lineRenderer.enabled is set to isGrappling
        {
            Vector3 grapplePosVec3 = new Vector3(grapplingPos.x, grapplingPos.y, transform.position.z);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePosVec3);
        }


        //will need to replace transform.right with mouse direction input
      
        

        
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            var directionToGrapplePos = (grapplingPos - (Vector2)transform.position).normalized;
            Debug.DrawRay(transform.position, directionToGrapplePos * speed, Color.white);
            rb.velocity = directionToGrapplePos * speed;
        }
    }
}
