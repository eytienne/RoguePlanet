using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiSpawner : MonoBehaviour
{
    void Update()
    {
        if (ActiveEnemies.activeEnemies <= 0)
        {
            for(int i = 0; i < ObjectPooler.SharedInstance.amountToPool ; i++)
            {
                waveManager();
                Debug.Log("spawn enemy nb : " + i);
            }
        }
        else
            Debug.Log(ActiveEnemies.activeEnemies);
    }

    void waveManager()
    {
        GameObject[] spawner = GameObject.FindGameObjectsWithTag("Spawner");
        Debug.Log("spawn");
        GameObject enemi = ObjectPooler.SharedInstance.GetPooledObject();
        Vector3 newPos = spawner[0].transform.position;
        newPos.x += ActiveEnemies.activeEnemies *5;
        if (enemi != null)
        {
            enemi.transform.position = newPos;
            enemi.transform.rotation = spawner[0].transform.rotation;
            enemi.SetActive(true);
            ActiveEnemies.activeEnemies += 1;
            Debug.Log(ActiveEnemies.activeEnemies);
        }
    }
}
