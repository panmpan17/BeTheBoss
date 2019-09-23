using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponeType { Laser, PlayerBullet, BossBullet, Minion }

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(Rigidbody2D))]
public class Weapone : Damageable
{
    [SerializeField]
    private WeaponeType type;

    private bool HasPool { get { return StaticHasPool(type); } }
    private static bool StaticHasPool(WeaponeType _type) {
        switch (_type) {
            case WeaponeType.Laser:
                return false;
            default:
                return true;
        }
    }

    private bool IsWallProof { get {
        switch (type) {
            case WeaponeType.Laser:
            case WeaponeType.Minion:
                return true;
            default:
                return false;
        }
    } }

    [SerializeField]
    private int damage;

    public static Weapone Spawn(WeaponeType _type) {
        if (!StaticHasPool(_type)) return null;
        return WeaponePrefabPool.GetPool(_type).Get();
    }

    public void Set(Vector3 pos, Vector2 vec) {
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vec;
        health = startingHealth;
    }

    public void Set(Vector3 pos, Vector2 vec, Quaternion rotation) {
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vec;
        transform.rotation = rotation;
        health = startingHealth;
    }

    public override void TakeDamage(int damage) {
        if (!HasPool) return;

        health -= damage;
        if (health < 0 && HasPool) WeaponePrefabPool.GetPool(type).Put(this);
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D other) {
        Damageable damageable = other.gameObject.GetComponent<Damageable>();
        if (IsWallProof && damageable == null) return;

        if (damageable != null) damageable.TakeDamage(damage);

        if (HasPool) WeaponePrefabPool.GetPool(type).Put(this);
    }

    public class WeaponePrefabPool {
        private static List<WeaponePrefabPool> prefabPools = new List<WeaponePrefabPool>();

        public static WeaponePrefabPool GetPool(WeaponeType _type) {
            for (int i = 0; i < prefabPools.Count; i++) {
                if (prefabPools[i].Type == _type) return prefabPools[i];
            }

            prefabPools.Add(new WeaponePrefabPool(_type));
            return prefabPools[prefabPools.Count - 1];
        }

        WeaponeType type;
        public WeaponeType Type { get { return type; } }
        GameObject prefab;
        List<Weapone> poolObjs;

        public WeaponePrefabPool(WeaponeType _type) {
            type = _type;
            poolObjs = new List<Weapone>();

            prefab = Resources.Load<GameObject>("Prefab/" + _type.ToString());
        }

        public Weapone Get() {
            Weapone weapone;
            if (poolObjs.Count > 0) {
                weapone = poolObjs[0];
                poolObjs.RemoveAt(0);
            } else { weapone = Instantiate(prefab).GetComponent<Weapone>(); }

            weapone.gameObject.SetActive(true);
            return weapone;
        }

        public void Put(Weapone weapone) {
            weapone.gameObject.SetActive(false);

            if (!poolObjs.Contains(weapone)) poolObjs.Add(weapone);
        }
    }
}
