using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    
    public GameObject titleScreen;
    public bool isGameActive;
    public TextMeshProUGUI speedrunTimerText;
    public bool timerActive = false;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        
    }

    public void StartGame()
    {
        isGameActive = true;
        titleScreen.SetActive(false);
        PlayerInControl.Invoke();
    }

    public void SetupSpeedrunTimer()
    {
        timerActive = true;
        speedrunTimerText.gameObject.SetActive(true);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timerActive) speedrunTimerText.text = GenerateTimeString();
    }

    private string GenerateTimeString()
    {
        int elapsedMs = (int)(timer * 1000);
        int minutes = Math.DivRem(elapsedMs, 60000, out int temp); // currently temp holds seconds * 1000 + ms
        int seconds = Math.DivRem(temp, 1000, out int ms);

        return $"{minutes:D2}:{seconds:D2}.{ms:D3}"; // xx:xx.xxx format
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public UnityEvent PlayerInControl;
}
