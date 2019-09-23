using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Rigidbody2D))]
public class BossBomb : Damageable
{
    static public List<BossBomb> pools = new List<BossBomb>();
    static private GameObject prefab = null;

    static public BossBomb Get()
    {
        if (prefab == null) prefab = Resources.Load<GameObject>("Prefab/BossBomb");

        BossBomb missle;
        if (pools.Count > 0)
        {
            missle = pools[0];
            pools.RemoveAt(0);
        }
        else missle = Instantiate(prefab).GetComponent<BossBomb>();

        missle.gameObject.SetActive(true);

        return missle;
    }

    static public void Put(BossBomb missle)
    {
        pools.Add(missle);
        missle.gameObject.SetActive(false);
    }

    bool exploding = false;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float readyExplodeFlySpeedMultiplier;
    private float flyTime;
    private bool readyExplode;

    // Start is called before the first frame update
    public void Setup(Vector3 pos, Vector2 vec, float _flyTime)
    {
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vec;
        flyTime = _flyTime;
        readyExplode = false;

        Animator anim = GetComponent<Animator>();
        anim.ResetTrigger("Explode");
        anim.ResetTrigger("ReadyExplode");
        anim.SetTrigger("Init");
    }

    private void FixedUpdate() {
        if (readyExplode) return;

        if (flyTime > 0) flyTime -= Time.deltaTime;
        else {
            GetComponent<Rigidbody2D>().velocity *= readyExplodeFlySpeedMultiplier;
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
        Put(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable damageable = other.GetComponent<Damageable>();
        Debug.Log(damageable);
        if (damageable) {
            damageable.TakeDamage(damage);
        }

        Explode();
    }

    public override void TakeDamage(int damage) {
        Explode();
    }
}
