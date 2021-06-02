using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int nbToSpawn = 10;
    private int nbSpawner = 10;

    Pool pool;

    void Awake() {
        pool = GetComponent<Pool>();
        
    }

    void FixedUpdate() {
        // Debug.Log("pool.inUseCount " + pool.inUseCount);
        if (pool.inUseCount < 1) {
            for(int i = 0; i < nbToSpawn; i++)
            {
                int rand = Random.Range(0, nbSpawner);
                // Debug.Log("rand " + rand);
                WaveManager(rand);
            }
        }
    }

    void WaveManager(int rand) {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        GameObject enemy = pool.GetObject();
        int rando = Random.Range(0, 1);
        if(rando == 0 || rando == 1)
        {
            enemy.GetComponent<Enemy>().setShootingAbility(true);
        }
        Vector3 newPos = spawners[rand].transform.position;
        newPos.x += pool.inUseCount * 5;
        // Debug.Log($"enemy {enemy} spawner {spawner.Length} newPos {newPos}");
        if (enemy != null) {
            enemy.transform.position = newPos;
            enemy.transform.rotation = spawners[rand].transform.rotation;
            enemy.SetActive(true);
        }
    }
}
