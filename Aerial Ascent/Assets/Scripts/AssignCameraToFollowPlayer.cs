using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class AssignCameraToFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera myCamera;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindWithTag("Player");
        myCamera.Follow = player.transform;
        myCamera.LookAt = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
