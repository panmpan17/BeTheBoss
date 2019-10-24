﻿#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReleaseVersion
{
    public enum WeaponType { Laser, PlayerBullet, PlayerMissle, BossBullet, Minion, BossBomb, Explosion }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Weapon : Damageable
    {
        [SerializeField]
        private WeaponType type;
        [SerializeField]
        protected int damage;
        [SerializeField]
        private bool putWhenTouchWall, putWhenDestory, putWhenTouchOpponent, useDamageRate;
        [SerializeField]
        private float damageRate;
        private bool triggerAnimationPlayed;
        [SerializeField]
        private AnimationEvent initAnimation, triggerAnimation, destroyAnimation;
        [SerializeField]
        private bool onlyOneDestroyEvent;
        [SerializeField]
        private DestroyEvent[] destroyRewards;
        private Timer damageRateTimer;
        private List<Damageable> contactDamaegable;

        private void Awake() {
            contactDamaegable = new List<Damageable>();
            if (useDamageRate) damageRateTimer = new Timer(damageRate);
        }

        private void Update()
        {
            if (useDamageRate)
            {
                if (damageRateTimer.UpdateEnd)
                {
                    damageRateTimer.Reset();
                    List<int> removedIndex = new List<int>();
                    for (int i = 0; i < contactDamaegable.Count; i++)
                    {
                        if (contactDamaegable[i].TakeDamage(damage)) {
                            removedIndex.Add(i);
                        }
                    }

                    removedIndex.Reverse();
                    for (int i = 0; i < removedIndex.Count; i++) contactDamaegable.RemoveAt(removedIndex[i]);
                }
            }
        }

        public void Setup(Vector3 pos, Vector2 velocity) {
            putting = false;
            contactDamaegable.Clear();
            health = startingHealth;
            transform.position = pos;
            GetComponent<Rigidbody2D>().velocity = velocity;

            if (initAnimation.BeenSet)
            {
                GetComponent<Animator>().SetTrigger(initAnimation.trigger);
            }
        }

        public void Setup(Vector3 pos, Vector2 velocity, Quaternion rotation)
        {
            putting = false;
            health = startingHealth;
            transform.position = pos;
            transform.rotation = rotation;
            GetComponent<Rigidbody2D>().velocity = velocity;

            if (initAnimation.BeenSet)
            {
                GetComponent<Animator>().SetTrigger(initAnimation.trigger);
            }
        }

        public override bool TakeDamage(int amount) {
            health -= amount;
            if (putWhenDestory && health <= 0) {
                if (destroyAnimation.BeenSet) {
                    GetComponent<Animator>().SetTrigger(destroyAnimation.trigger);
                } else {
                    Debug.Log("destroy put");
                    PrepareToPut();
                }
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null) {
                contactDamaegable.Add(damageable);
                damageable.TakeDamage(damage);
            }

            if (putWhenTouchWall && other.gameObject.layer == GameManager.WallLayer) PrepareToPut();
            if (putWhenTouchOpponent && (other.gameObject.layer == GameManager.PlayerLayer || other.gameObject.layer == GameManager.BossLayer)) PrepareToPut();
        }

        private void OnTriggerExit2D(Collider2D other) {
            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null)
            {
                contactDamaegable.Remove(damageable);
            }
        }

        private IEnumerator DelayExecute(float sec, System.Action action) {
            yield return new WaitForSeconds(sec);
            action();
        }

        private void SpawnDestroyEvent() {
            for (int i = 0; i < destroyRewards.Length; i++) {
                float chance = Random.Range(0f, 1f);
                if (chance <= destroyRewards[i].chance) {
                    Vector3 vec = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));
                    switch (destroyRewards[i].type) {
                        case DestroyEventType.MedPack:
                            RewardPrefabPool.GetPool(RewardType.MedPack).GetFromPool().Setup(transform.position, vec.normalized * 2);
                            break;
                        case DestroyEventType.MissileSupply:
                            RewardPrefabPool.GetPool(RewardType.MissileSupply).GetFromPool().Setup(transform.position, vec.normalized * 2);
                            break;
                        case DestroyEventType.Explode:
                            Weapon explosion = WeaponPrefabPool.GetPool(WeaponType.Explosion).GetFromPool();
                            explosion.Setup(transform.position, Vector3.zero);
                            explosion.StartCoroutine(DelayExecute(1, delegate {
                                explosion.GetComponent<Animator>().SetTrigger("End");
                            }));
                            break;
                    }

                    if (onlyOneDestroyEvent) break;
                }
            }
        }

        private bool putting;
        private void PrepareToPut() {
            if (putting) return;
            putting = true;
            if (destroyRewards.Length > 0) SpawnDestroyEvent();

            if (triggerAnimation.BeenSet)GetComponent<Animator>().SetTrigger(triggerAnimation.trigger);
            else WeaponPrefabPool.GetPool(type).PutWeapon(this);
        }

        void OnTriggerAnimationEnd() {
            Debug.Log("animation done put");
            WeaponPrefabPool.GetPool(type).PutWeapon(this);
        }

        private void OnCollisionEnter2D(Collision2D other) {OnTriggerEnter2D(other.collider); }
        private void OnCollisionStay2D(Collision2D other) { OnTriggerEnter2D(other.collider); }
        private void OnCollisionExit2D(Collision2D other) { OnTriggerExit2D(other.collider); }

        [System.Serializable]
        private class AnimationEvent {
            public string trigger;
            public bool BeenSet { get { return trigger != ""; } }
        }

        [System.Serializable]
        private class DestroyEvent {
            public DestroyEventType type;
            public float chance;
        }

        public enum DestroyEventType { MedPack, Explode, MissileSupply }
    }

    public class WeaponPrefabPool {
        static List<WeaponPrefabPool> prefabPools = new List<WeaponPrefabPool>();

        public static WeaponPrefabPool GetPool(WeaponType _type) {
            for (int i = 0; i < prefabPools.Count; i++) {
                if (prefabPools[i].Type == _type) return prefabPools[i];
            }

            prefabPools.Add(new WeaponPrefabPool(_type));
            return prefabPools[prefabPools.Count - 1];
        }

        public static Weapon GetFromPool(WeaponType _type)
        {
            for (int i = 0; i < prefabPools.Count; i++)
            {
                if (prefabPools[i].Type == _type) return prefabPools[i].GetFromPool();
            }

            prefabPools.Add(new WeaponPrefabPool(_type));
            return prefabPools[prefabPools.Count - 1].GetFromPool();
        }

        public static void ClearAllPool() {
            for (int i = 0; i < prefabPools.Count; i++)
            {
                prefabPools[i].Clear();
            }
        }

        private WeaponType type;
        public WeaponType Type { get { return type; } }
        private GameObject prefab;
        private List<Weapon> poolObjs;
        public List<Weapon> AliveObjects;

        public void Clear() {
            poolObjs.Clear();
            AliveObjects.Clear();
        }

        public WeaponPrefabPool(WeaponType _type) {
            type = _type;
            poolObjs = new List<Weapon>();
            AliveObjects = new List<Weapon>();

            prefab = Resources.Load<GameObject>("Prefab/" + _type.ToString());
        }

        public Weapon GetFromPool() {
            Weapon Weapon;
            if (poolObjs.Count > 0) {
                Weapon = poolObjs[0];
                poolObjs.RemoveAt(0);
            } else { Weapon = GameObject.Instantiate(prefab).GetComponent<Weapon>(); }

            Weapon.gameObject.SetActive(true);
            AliveObjects.Add(Weapon);
            return Weapon;
        }

        public void PutWeapon(Weapon Weapon) {
            Weapon.gameObject.SetActive(false);
            AliveObjects.Remove(Weapon);

            if (!poolObjs.Contains(Weapon)) poolObjs.Add(Weapon);
        }
    }
}