using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public int nbToSpawn = 10;
    public int nbWave = 0;
    public Text nbWaveT;
    
    Pool pool;
    GameObject[] spawners;

    void Awake() {
        pool = GetComponent<Pool>();
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }

    void FixedUpdate() {
        if (pool.inUseCount < 1 && nbToSpawn > 0) {
            for (int i = 0; i < nbToSpawn; i++) {
                int spawnerIndex = Random.Range(0, spawners.Length - 1);
                WaveManager(spawnerIndex);
            }
            nbWaveT.text = nbWave.ToString();
            //new Bonus().spawnBonus();
            nbWave += 1;
            nbToSpawn += 5;
        }
    }

    void WaveManager(int spawnerIndex) {
        GameObject enemy = pool.GetObject();
        bool rand = Random.Range(0f, 1f) > 0.2f;
        if(rand)
        {
            enemy.GetComponent<Enemy>().SetShootingAbility(true);
        }
        if (enemy != null) {
            Vector3 newPos = spawners[spawnerIndex].transform.position;
            newPos.x += pool.inUseCount * 5;
            enemy.transform.position = newPos;
            enemy.transform.rotation = spawners[spawnerIndex].transform.rotation;
            enemy.SetActive(true);
        }
    }
}
