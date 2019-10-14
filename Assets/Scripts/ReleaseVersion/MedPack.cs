using UnityEngine;

namespace ReleaseVersion
{
    public enum DestroyRewardType { MedPack }    

    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class MedPack : MonoBehaviour
    {
        static public PrefabPoolCtrl<MedPack> Pools = new PrefabPoolCtrl<MedPack>();

        [SerializeField]
        private int healthReward;

        public void Setup(Vector3 pos, Vector2 vec) {
            transform.position = pos;
            GetComponent<Rigidbody2D>().velocity = vec;
        }

        // private void OnTriggerEnter2D(Collider2D other) {
        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer != GameManager.PlayerLayer) return;
            other.gameObject.GetComponent<PlayerContoller>().AddHealth(healthReward);
            Pools.PutAliveObject(this);
        }
    }
}