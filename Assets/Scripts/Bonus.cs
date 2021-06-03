using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public Player player;
    AudioSource audioData;
    public GameObject prefab;

    void Awake() {
        audioData = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "Player") {
            float i = Random.Range(0, 1.0f);
            if (i < 0.4f)
            {
                player.life += 10;
            }
            else if (i>=0.4f && i < 0.8f)
            {
                if (player.fireRate > 0.3) player.fireRate += 0.05f;
                else if (player.fireRate > 0.1) player.fireRate += 0.02f;

            }
            else if (i >= 0.8f)
            {
                player.bulletnumber += 1;
            }
            Debug.Log("Touché par le joueur");
            AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
            this.gameObject.SetActive(false);
        }   
    }
    /*
    public void spawnBonus()
    {
        GameObject clone;
        clone = Instantiate(prefab, player.transform.position,player.transform.rotation);
    }*/
}
