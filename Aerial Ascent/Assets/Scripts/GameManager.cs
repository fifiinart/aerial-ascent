using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
    public GameObject titleScreen;
    public bool isGameActive;

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
