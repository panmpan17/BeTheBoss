using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField]
    protected int startingHealth;
    protected int health;

    protected void SetupHealth()
    {
        health = startingHealth;
    }

    public abstract void TakeDamage(int damage);
}

public class PrefabPoolCtrl<T> where T : MonoBehaviour {
    private List<T> poolObjects;
    private List<T> aliveObjects;
    private T prefab;

    public List<T> PoolObjects { get { return poolObjects; } }
    public List<T> AliveObjects { get { return aliveObjects; } }

    public PrefabPoolCtrl(T _prefab) {
        prefab = _prefab;
        poolObjects = new List<T>();
        aliveObjects = new List<T>();
    }

    public T GetFromPool() {
        T obj = null;

        if (poolObjects.Count > 0) {
            obj = poolObjects[0];
            poolObjects.RemoveAt(0);
        } else obj = GameObject.Instantiate(prefab);

        aliveObjects.Add(obj);
        obj.gameObject.SetActive(true);
        return null;
    }

    public void PutAliveObject(T obj) {
        obj.gameObject.SetActive(false);
        aliveObjects.Remove(obj);
        poolObjects.Add(obj);
    }
}