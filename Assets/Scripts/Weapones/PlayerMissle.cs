using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerMissle : Damageable
{
    static public PrefabPoolCtrl<PlayerMissle> Pools = new PrefabPoolCtrl<PlayerMissle>();

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
        PlayerContoller.ins.MissleEnd();
        Pools.PutAliveObject(this);
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
