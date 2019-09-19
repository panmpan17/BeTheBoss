using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerMissle : Damageable
{
    static public List<PlayerMissle> pools = new List<PlayerMissle>();
    static private GameObject prefab = null;

    static public PlayerMissle Get() {
        if (prefab == null) prefab = Resources.Load<GameObject>("Prefab/PlayerMissle");

        PlayerMissle missle;
        if (pools.Count > 0) {
            missle = pools[0];
            pools.RemoveAt(0);
        } else missle = Instantiate(prefab).GetComponent<PlayerMissle>();

        missle.gameObject.SetActive(true);

        return missle;
    } 

    static public void Put(PlayerMissle missle) {
        pools.Add(missle);
        missle.gameObject.SetActive(false);
    }

    bool exploding = false;
    [SerializeField]
    private int damage;

    // Start is called before the first frame update
    public void Setup(Vector3 pos, Vector2 vec)
    {
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vec;
        GetComponent<Animator>().SetTrigger("Init");
    }

    void Explode() {
        if (exploding) return;

        exploding = true;
        GetComponent<Animator>().SetTrigger("Explode");
    }

    void ExplodeEnd() {
        exploding = false;
        Put(this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Explode();

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable) {
            damageable.TakeDamage(damage);
        }
    }

    public override void TakeDamage(int damage)
    {
        Explode();
    }
}
