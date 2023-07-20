using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private CinemachineBrain myCB;
    public float shakeLeft = 0;

    public float time;
    public float amplitude;

    // Start is called before the first frame update
    void Start()
    {
        myCB = Camera.main.GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeLeft > 0)
        {
            shakeLeft -= Time.deltaTime;
        }
        else
        {
            SetAmplitude(0f);
        }
    }

    public void cameraShake(bool on = true)
    {
        SetAmplitude(amplitude);
        shakeLeft = time;
    }

    private void SetAmplitude(float amplitude)
    {
        cam = myCB.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (cam != null) {
            var perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = amplitude;
        }
    }
}
