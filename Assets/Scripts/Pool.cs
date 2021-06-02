using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class Pool : MonoBehaviour, ISerializationCallbackReceiver
{
    public GameObject model;
    public int amount;
    public bool allowGettingUsedWhenNoAvailable = false;

    [SerializeField]
    List<Pooled> _inUse;
    LinkedList<Pooled> inUse;
    [SerializeField]
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

    public void OnBeforeSerialize() {
        _inUse = inUse.ToList();
    }

    public void OnAfterDeserialize() {
        if (_inUse != null) {
            inUse = new LinkedList<Pooled>(_inUse);
        } else {
            inUse = new LinkedList<Pooled>();
        }
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
