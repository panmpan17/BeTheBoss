#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Rigidbody2D))]
public class BossBomb : Damageable
{
    static public PrefabPoolCtrl<BossBomb> Pools = new PrefabPoolCtrl<BossBomb>();

    bool exploding = false;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float readyExplodeFlySpeedMultiplier;
    private float flyTime;
    private bool readyExplode;
    private new Rigidbody2D rigidbody2D;
    private List<Damageable> damagedSend;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector3 pos, Quaternion rotation, Vector2 vec, float _flyTime)
    {
        damagedSend = new List<Damageable>();
        transform.position = pos;
        transform.rotation = rotation;
        rigidbody2D.velocity = vec;
        flyTime = _flyTime;
        readyExplode = false;

        Animator anim = GetComponent<Animator>();
        anim.ResetTrigger("Explode");
        anim.ResetTrigger("ReadyExplode");
        anim.SetTrigger("Init");
    }

    private void FixedUpdate() {
        if (readyExplode) return;

        if (flyTime > 0) {
            flyTime -= Time.fixedDeltaTime;

            // Vector3 vectorToTarget = attractPos - transform.position;
            // float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
            // rigidbody2D.AddForceAtPosition(vectorToTarget * torque, attractPos);
            // transform.rotation = Quaternion.RotateTowards(
            //     transform.rotation,
            //     Quaternion.AngleAxis(
            //         Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90,
            //         Vector3.forward),
            //     torque);
        } else {
            rigidbody2D.velocity *= readyExplodeFlySpeedMultiplier;
            readyExplode = true;
            GetComponent<Animator>().SetTrigger("ReadyExplode");
        }
    }

    void Explode()
    {
        if (exploding) return;

        readyExplode = false;
        exploding = true;
        GetComponent<Animator>().SetTrigger("Explode");
    }

    void ExplodeEnd()
    {
        exploding = false;
        Pools.PutAliveObject(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable) {
            if (!damagedSend.Contains(damageable)) {
                damagedSend.Add(damageable);
                damageable.TakeDamage(damage);
            }
        }

        Explode();
    }

    public override bool TakeDamage(int damage) {
        Explode();
        return true;
    }
}
