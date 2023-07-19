using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
    public GameObject titleScreen;
    public bool isGameActive;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        
    }

    public void StartGame()
    {
        isGameActive = true;
        titleScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UnityEvent PlayerInControl;
}
