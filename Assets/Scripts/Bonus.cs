using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    AudioSource audioData;

    void Awake() {
        audioData = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "Player") {
            Debug.Log("Touché par le joueur");
            AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
            this.gameObject.SetActive(false);
        }
    }
}
