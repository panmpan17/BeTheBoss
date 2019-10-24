#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

namespace ReleaseVersion
{
    public enum RewardType { MedPack, MissileSupply }

    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Reward : MonoBehaviour
    {
        [SerializeField]
        private RewardType type;
        [SerializeField]
        private int arg;

        public void Setup(Vector3 pos, Vector2 vec)
        {
            transform.position = pos;
            GetComponent<Rigidbody2D>().velocity = vec;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != GameManager.PlayerLayer) return;

            switch (type) {
                case RewardType.MedPack:
                    other.gameObject.GetComponent<PlayerContoller>().AddHealth(arg);
                    break;
                case RewardType.MissileSupply:
                    other.gameObject.GetComponent<PlayerContoller>().AddMissile(arg);
                    break;
            }
            
            RewardPrefabPool.GetPool(type).PutReward(this);
        }
    }

    public class RewardPrefabPool {
        static List<RewardPrefabPool> prefabPools = new List<RewardPrefabPool>();

        static public RewardPrefabPool GetPool(RewardType _type) {
            for (int i = 0; i < prefabPools.Count; i++)
            {
                if (prefabPools[i].Type == _type) return prefabPools[i];
            }

            prefabPools.Add(new RewardPrefabPool(_type));
            return prefabPools[prefabPools.Count - 1];
        }

        private RewardType type;
        public RewardType Type { get { return type; } }
        private GameObject prefab;
        private List<Reward> poolObjs;
        public List<Reward> AliveObjects;

        public void Clear()
        {
            poolObjs.Clear();
            AliveObjects.Clear();
        }

        public RewardPrefabPool(RewardType _type)
        {
            type = _type;
            poolObjs = new List<Reward>();
            AliveObjects = new List<Reward>();

            prefab = Resources.Load<GameObject>("Prefab/" + _type.ToString());
        }

        public Reward GetFromPool()
        {
            Reward _reward;
            if (poolObjs.Count > 0)
            {
                _reward = poolObjs[0];
                poolObjs.RemoveAt(0);
            }
            else { _reward = GameObject.Instantiate(prefab).GetComponent<Reward>(); }

             _reward.gameObject.SetActive(true);
            AliveObjects.Add(_reward);
            return _reward;
        }

        public void PutReward(Reward _reward)
        {
            _reward.gameObject.SetActive(false);
            AliveObjects.Remove(_reward);

            if (!poolObjs.Contains(_reward)) poolObjs.Add(_reward);
        }
    }
}