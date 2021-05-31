using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    AudioSource audioData;
    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
            Debug.Log("Touché par le joueur");
            this.gameObject.SetActive(false);
        }
    }
}
