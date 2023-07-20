using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{

    public float speed = 20f;
    public float distance = 10f;
    RaycastHit2D hit;
    public LayerMask groundMask;
    public LayerMask canGrappleMask;
    public LayerMask cannotGrappleMask;
    public LineRenderer lineRenderer;
    private Vector2 grapplingPos;
    public bool isGrappling = false;
    public float grappleAngle = 0f;
    public float actualSpeed = 0f;
    public Vector2 directionToGrapplePos;
    private Rigidbody2D rb;
    public bool inControl;
    private PlayerController playerController;
    public CameraShake camShake;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();   
        rb = GetComponent<Rigidbody2D>();
        inControl = false;
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

            if (Input.GetMouseButtonDown(0) && isGrappling == false && inControl)
            {
                hit = Physics2D.Raycast(transform.position, lookDirection, distance, canGrappleMask);
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ground") || hit.transform.gameObject.layer == LayerMask.NameToLayer("CanGrapple"))
                {
<<<<<<< HEAD

                    StartGrappling();
                }

                //hit = Physics2D.Raycast(transform.position, lookDirection, distance, cannotGrappleMask);
                //hit = Physics2D.Raycast(transform.position, lookDirection, distance, cannotGrappleMask);
                //if (!hit)
                //{
                //    camShake.cameraShake();
                //        hit = Physics2D.Raycast(transform.position, lookDirection, distance, groundMask);
                //        if (hit)
                //        {
                //            StartGrappling();
                //        }
                    
                //}
=======
                    camShake.cameraShake();
                    StartGrappling();
                }
                else
                {
                    hit = Physics2D.Raycast(transform.position, lookDirection, distance, groundMask);
                    if (hit)
                    {
                        StartGrappling();
                        camShake.cameraShake();

                    }
                }
>>>>>>> b9c4f0b1881c79338e0da37a421a166761d0a786
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

    public void PlayerInControl()
    {
        print("in control");
        // ...give player ability to control grappling hook
        inControl = true;

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
        Vector2 _ = new Vector2(Mathf.Abs(directionToGrapplePos.x), directionToGrapplePos.y);
        grappleAngle = Vector2.Angle(Vector2.right, _);
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

