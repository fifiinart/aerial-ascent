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
    public float grappleAngle = 0f;
    public float actualSpeed = 0f;
    public Vector2 directionToGrapplePos;
    private Rigidbody2D rb;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();   
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.gameOver)
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
                    StartGrappling();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopGrappling();
            }
            if (lineRenderer.enabled = isGrappling) // lineRenderer.enabled is set to isGrappling
            {
                Vector3 grapplePosVec3 = new Vector3(grapplingPos.x, grapplingPos.y, transform.position.z);
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, grapplePosVec3);
            }
        }
    }

    public void StopGrappling()
    {
        //stop Grappling
        isGrappling = false;

        grappleAngle = 0f;
    }

    public void StartGrappling()
    {
        grapplingPos = hit.point;
        isGrappling = true;

        directionToGrapplePos = grapplingPos - (Vector2)transform.position;
        grappleAngle = Vector2.Angle(transform.rotation.eulerAngles, directionToGrapplePos);
        float dotProduct = Vector2.Dot(rb.velocity, directionToGrapplePos.normalized);
        actualSpeed = Mathf.Max(speed, dotProduct); // maintain rb velocity if we have more than speed
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            Debug.DrawRay(transform.position, directionToGrapplePos, Color.white);
            rb.velocity = directionToGrapplePos.normalized * actualSpeed;
        }
    }
}
