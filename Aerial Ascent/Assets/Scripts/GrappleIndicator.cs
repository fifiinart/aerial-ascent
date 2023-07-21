using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleIndicator : MonoBehaviour
{

    public GameObject orgin;
    public Grappling grapple;
    private SpriteRenderer sr;
    public Color canGrapple = new Color(0,255,0);
    public Color cannotGrapple = new Color(255, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (grapple.CanGrapple()) 
        {
            sr.color = canGrapple;
        } else
        {
            sr.color = cannotGrapple;
        }
    }
}
