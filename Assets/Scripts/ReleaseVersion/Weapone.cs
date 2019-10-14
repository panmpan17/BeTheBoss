using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReleaseVersion
{
    public enum WeaponeType { Laser, PlayerBullet, PlayerMissle, BossBullet, Minion, BossBomb }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Weapone : Damageable
    {
        [SerializeField]
        private WeaponeType type;
        [SerializeField]
        protected int damage;
        [SerializeField]
        private bool putWhenTouchWall, putWhenDestory, putWhenTouchOpponent;
        private bool triggerAnimationPlayed;
        [SerializeField]
        private AnimationEvent initAnimation, triggerAnimation, destroyAnimation;
        [SerializeField]
        private bool onlyOneDestroyReward;
        [SerializeField]
        private DestroyReward[] destroyRewards;

        // public delegate void TestDelegate(int value);

        // private void Awake() {
        //     TestDelegate deleg = null;
        //     deleg += delegate (int value) {};
        //     deleg.Invoke(2);
        // }

        public void Setup(Vector3 pos, Vector2 velocity) {
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
            health = startingHealth;
            transform.position = pos;
            transform.rotation = rotation;
            GetComponent<Rigidbody2D>().velocity = velocity;

            if (initAnimation.BeenSet)
            {
                GetComponent<Animator>().SetTrigger(initAnimation.trigger);
            }
        }

        public override void TakeDamage(int amount) {
            health -= amount;
            if (putWhenDestory && health <= 0) {
                if (destroyAnimation.BeenSet) {
                    GetComponent<Animator>().SetTrigger(destroyAnimation.trigger);
                } else {
                    PrepareToPut();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (putWhenTouchWall && other.gameObject.layer == GameManager.WallLayer) PrepareToPut();
            if (putWhenTouchOpponent && (other.gameObject.layer == GameManager.PlayerLayer || other.gameObject.layer == GameManager.BossLayer)) PrepareToPut();

            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null) {
                damageable.TakeDamage(damage);
            }
        }

        private void SpawnDestroyReward() {
            for (int i = 0; i < destroyRewards.Length; i++) {
                float chance = Random.Range(0f, 1f);
                if (chance <= destroyRewards[i].chance) {
                    switch (destroyRewards[i].type) {
                        case DestroyRewardType.MedPack:
                            Vector3 vec = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));
                            MedPack.Pools.GetFromPool().Setup(transform.position, vec.normalized * 2);
                            break;
                    }

                    if (onlyOneDestroyReward) break;
                }
            }
        }

        private void PrepareToPut() {
            if (destroyRewards.Length > 0) SpawnDestroyReward();

            if (triggerAnimation.BeenSet)GetComponent<Animator>().SetTrigger(triggerAnimation.trigger);
            else WeaponePrefabPool.GetPool(type).PutWeapone(this);
        }

        void OnTriggerAnimationEnd() {
            WeaponePrefabPool.GetPool(type).PutWeapone(this);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            OnTriggerEnter2D(other.collider);
        }

        [System.Serializable]
        private class AnimationEvent {
            public string trigger;
            public bool BeenSet { get { return trigger != ""; } }
        }

        [System.Serializable]
        private class DestroyReward {
            public DestroyRewardType type;
            public float chance;
        }
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

        public static void ClearAllPool() {
            for (int i = 0; i < prefabPools.Count; i++)
            {
                prefabPools[i].Clear();
            }
        }

        private WeaponeType type;
        public WeaponeType Type { get { return type; } }
        private GameObject prefab;
        private List<Weapone> poolObjs;
        public List<Weapone> AliveObjects;

        public void Clear() {
            poolObjs.Clear();
            AliveObjects.Clear();
        }

        public WeaponePrefabPool(WeaponeType _type) {
            type = _type;
            poolObjs = new List<Weapone>();
            AliveObjects = new List<Weapone>();

            prefab = Resources.Load<GameObject>("Prefab/" + _type.ToString());
        }

        public Weapone GetFromPool() {
            Weapone weapone;
            if (poolObjs.Count > 0) {
                weapone = poolObjs[0];
                poolObjs.RemoveAt(0);
            } else { weapone = GameObject.Instantiate(prefab).GetComponent<Weapone>(); }

            weapone.gameObject.SetActive(true);
            AliveObjects.Add(weapone);
            return weapone;
        }

        public void PutWeapone(Weapone weapone) {
            weapone.gameObject.SetActive(false);
            AliveObjects.Remove(weapone);

            if (!poolObjs.Contains(weapone)) poolObjs.Add(weapone);
        }
    }
}