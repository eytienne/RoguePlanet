using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public Player player;
    public GameObject PauseCanvas;
    public GameObject OptionsCanvas;
    bool Paused = false;

    public void Pause()
    {
        if (OptionsCanvas.activeSelf == false)
        {
            if (Paused)
            {
                PauseCanvas.SetActive(false);
                Time.timeScale = 1f;
                Debug.Log("desactiver");
                Paused = false;
            }
            else
            {
                PauseCanvas.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("activer");
                Paused = true;
            }
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
