using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public Player player;
    public GameObject PauseCanvas;
    public GameObject OptionsCanvas;
    float currentTimeScale;
    void Start()
    {
        player.inputActions.Player.Pause.performed += _ => Pause();
    }
    public void Pause()
    {
        if (Time.timeScale == 0f)
        {
            PauseCanvas.SetActive(false);
            Time.timeScale = currentTimeScale;
            Debug.Log("desactiver");
        }
        else
        {
            PauseCanvas.SetActive(true); 
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Debug.Log("activer");
        }
    }
    public void Quitter()
    {
        Application.Quit();
    }
    public void Options()
    {
        if (PauseCanvas.activeSelf)
        {
            PauseCanvas.SetActive(false);
            OptionsCanvas.SetActive(true);
        }
        else
        {
            PauseCanvas.SetActive(true);
            OptionsCanvas.SetActive(false);
        }
        
    }
}
