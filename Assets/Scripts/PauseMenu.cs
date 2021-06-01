using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject Canvas;
    float currentTimeScale;

    public void Pause() {
        Debug.Log("Pause()");
        if (Time.timeScale == 0f) {
            Canvas.SetActive(false);
            Time.timeScale = currentTimeScale;
            Debug.Log("desactiver");
        } else {
            Canvas.SetActive(true);
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Debug.Log("activer");
        }
    }

    public void Quitter() {
        Application.Quit();
    }
}
