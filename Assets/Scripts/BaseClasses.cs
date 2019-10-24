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

    public abstract bool TakeDamage(int damage);
}

public class PrefabPoolCtrl<T> where T : MonoBehaviour {
    private List<T> poolObjects;
    private List<T> aliveObjects;
    private GameObject prefab;
    
    public delegate void InstantiateEvent(T component);
    private InstantiateEvent instantiateEvents = null;

    public List<T> PoolObjects { get { return poolObjects; } }
    public List<T> AliveObjects { get { return aliveObjects; } }

    public PrefabPoolCtrl()
    {
        poolObjects = new List<T>();
        aliveObjects = new List<T>();
    }

    public void SetupPrefab(GameObject _prefab)
    {
        prefab = _prefab;
    }
    public void AddInstantiateEvent(InstantiateEvent action)
    {
        instantiateEvents += action;
    }

    public void SetupPrefab(string prefabResourcePath)
    {
        GameObject obj = Resources.Load<GameObject>(prefabResourcePath);
        if (obj == null) {
            Debug.LogError("Path don't exist");
            return;
        }

        if (obj.GetComponent<T>() == null) {
            Debug.LogError("Prefab don't have this component");
            return;
        }

        prefab = obj;
    }

    public void Clear() {
        instantiateEvents = null;
        aliveObjects.Clear();
        poolObjects.Clear();
    }

    public T GetFromPool() {
        T component = null;

        if (poolObjects.Count > 0) {
            component = poolObjects[0];
            poolObjects.RemoveAt(0);
        } else {
            component = GameObject.Instantiate(prefab).GetComponent<T>();
            if (instantiateEvents != null) instantiateEvents(component);
        }

        aliveObjects.Add(component);
        component.gameObject.SetActive(true);
        return component;
    }

    public void PutAliveObject(T component) {
        Debug.Log(component, component);
        component.gameObject.SetActive(false);
        aliveObjects.Remove(component);
        poolObjects.Add(component);
    }
}