using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BulletsPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static BulletsPool Instance;

    void OnEnable() {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionnary;

    void Start() {
        poolDictionnary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionnary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion? rotation = null) {
        if (!poolDictionnary.ContainsKey(tag)) {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist");
            return null;
        }

        GameObject toSpawn = poolDictionnary[tag].Dequeue();

        toSpawn.SetActive(true);
        toSpawn.transform.SetPositionAndRotation(position, rotation ?? Quaternion.identity);

        poolDictionnary[tag].Enqueue(toSpawn);

        return toSpawn;
    }
}
