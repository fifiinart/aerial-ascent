using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleIndicator : MonoBehaviour
{

    public GameObject orgin;
    public Grappling grapple;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDirection = (mousePos - orgin.transform.position).normalized;
        orgin.transform.rotation = Quaternion.Euler(lookDirection);
        if(grapple.) { }
    }
}
