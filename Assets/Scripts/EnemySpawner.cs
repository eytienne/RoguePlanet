using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int nbToSpawn = 10;

    Pool pool;

    void Awake() {
        pool = GetComponent<Pool>();
    }

    void FixedUpdate() {
        // Debug.Log("pool.inUseCount " + pool.inUseCount);
        if (pool.inUseCount < 1) {
            int rand = Random.Range(0, nbToSpawn);
            // Debug.Log("rand " + rand);
            WaveManager(0);
        }
    }

    void WaveManager(int rand) {
        GameObject[] spawner = GameObject.FindGameObjectsWithTag("Spawner");
        GameObject enemy = pool.GetObject();
        Vector3 newPos = spawner[rand].transform.position;
        newPos.x += pool.inUseCount * 5;
        // Debug.Log($"enemy {enemy} spawner {spawner.Length} newPos {newPos}");
        if (enemy != null) {
            enemy.transform.position = newPos;
            enemy.transform.rotation = spawner[rand].transform.rotation;
            enemy.SetActive(true);
        }
    }
}
