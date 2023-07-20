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

    private LineRenderer lineRenderer;
    private TrailRenderer tr;
    private Vector2 grapplingPos;

    [HideInInspector]
    public bool isGrappling = false;
    [HideInInspector]
    public float grappleAngle = 0f;
    [HideInInspector]
    public float actualSpeed = 0f;
    [HideInInspector]
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
        tr = GetComponent<TrailRenderer>();
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

                if (hit && (hit.transform.gameObject.layer == LayerMask.NameToLayer("ground") || hit.transform.gameObject.layer == LayerMask.NameToLayer("CanGrapple")))
                {
                    camShake.cameraShake();
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

    public void PlayerInControl()
    {
        inControl = true;

    }

    public void PlayerWin()
    {
        //...
    }

    public void StopGrappling()
    {
        //stop Grappling
        isGrappling = false;

        grappleAngle = 0f;
        tr.emitting = false;
    }

    public void StartGrappling()
    {
        grapplingPos = hit.point;
        isGrappling = true;

        directionToGrapplePos = grapplingPos - (Vector2)transform.position;
        Vector2 _ = new Vector2(Mathf.Abs(directionToGrapplePos.x), directionToGrapplePos.y);
        grappleAngle = Vector2.Angle(Vector2.right, _);
        float dotProduct = Vector2.Dot(rb.velocity, directionToGrapplePos.normalized);
        actualSpeed = Mathf.Max(speed, dotProduct);
        tr.Clear();
        tr.emitting = true;

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

