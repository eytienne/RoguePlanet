using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Pooled : MonoBehaviour
{
    public Pool origin;

    void OnDisable() {
        origin.ReleaseObject(this);
    }
}

[Serializable]
public class NoAvailableObjectException : Exception
{
    public NoAvailableObjectException() { }
}

public class Pool : MonoBehaviour
{
    public GameObject model;
    public int amount;
    public bool allowGettingUsedWhenNoAvailable = false;

    LinkedList<Pooled> inUse = new LinkedList<Pooled>();
    List<Pooled> available;

    public int availableCount {
        get {
            return available.Count;
        }
    }
    public int inUseCount {
        get {
            return inUse.Count;
        }
    }

    bool _isGameObject;
    public Pool() {
        available = new List<Pooled>(amount);
    }

    void Awake() {
        for (int i = 0; i < amount; i++) {
            GameObject instance = Instantiate(model);
            instance.SetActive(false);
            Pooled pooled = instance.AddComponent<Pooled>();
            pooled.origin = this;
            available.Add(pooled);
        }
    }

    public GameObject GetObject() {
        return GetPooledObject().gameObject;
    }

    public Pooled GetPooledObject() {
        Pooled instance;
        if (available.Count > 0) {
            instance = available.Pop();
        } else if (allowGettingUsedWhenNoAvailable) {
            instance = inUse.First.Value;
            inUse.RemoveFirst();
        } else {
            throw new NoAvailableObjectException();
        }
        inUse.AddLast(instance);
        return instance;
    }

    public void ReleaseObject(Pooled pooled) {
        bool removed = inUse.Remove(pooled);
        if (!removed) throw new ArgumentException("Not in the pool", "pooled");
        available.Add(pooled);
    }
}
