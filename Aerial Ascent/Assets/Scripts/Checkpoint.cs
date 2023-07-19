using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool used = false;
    private CircleCollider2D cc;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CircleCollider2D>();
    }

    public void Activate()
    {
        used = true;
        cc.enabled = false;
        anim.SetTrigger("CheckpointActivate");
    }
}
