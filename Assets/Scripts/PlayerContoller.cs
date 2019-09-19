﻿#define SHOOTING

using System.Collections;
using UnityEngine;

public abstract class Damageable : MonoBehaviour {
    [SerializeField]
    protected int startingHealth;
    protected int health;

    protected void SetupHealth () {
        health = startingHealth;
    }

    public abstract void TakeDamage(int damage);
}

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerContoller : Damageable
{
    private bool Left { get { return Input.GetKey(KeyCode.LeftArrow); } }
    private bool Right { get { return Input.GetKey(KeyCode.RightArrow); } }
    private bool Down { get { return Input.GetKey(KeyCode.DownArrow); } }
    private bool Up { get { return Input.GetKey(KeyCode.UpArrow); } }
    [SerializeField]
    private float speed, fireRate, bulletSpeed;
    private float fireRateCount;
    [SerializeField]
    private Sprite[] leftSprite, rightSprite;
    [SerializeField]
    private Sprite idleSprite;
    [SerializeField]
    private Transform[] burstPosition;

    private State state;
    private enum State { Left, Idle, Right }

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid2d;

    void Awake() {
        SetupHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        fireRateCount += Time.deltaTime;
        if (fireRateCount > fireRate) {
            fireRateCount = 0;
        #if SHOOTING
            for (int i = 0; i < burstPosition.Length; i++) Weapone.Spawn(WeaponeType.PlayerBullet).Set(burstPosition[i].position, Vector2.up * bulletSpeed);
        #endif
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayerMissle missle = PlayerMissle.Get();
            missle.Setup(transform.position, Vector2.up);
        }
    }

    void FixedUpdate() {
        State newState = Left ? State.Left: (Right? State.Right: State.Idle);
        if (state != newState) {
            StopAllCoroutines();
            state = newState;
            switch (state) {
                case State.Left:
                    StartCoroutine(FlipLeft());
                    rigid2d.velocity = new Vector2(-speed, 0);
                    break;
                case State.Right:
                    StartCoroutine(FlipRight());
                    rigid2d.velocity = new Vector2(speed, 0);
                    break;
                case State.Idle:
                    rigid2d.velocity = Vector2.zero;
                    spriteRenderer.sprite = idleSprite;
                    break;
            }
        }

        if (Up) rigid2d.velocity = new Vector2(rigid2d.velocity.x, speed);
        else if (Down) rigid2d.velocity = new Vector2(rigid2d.velocity.x, -speed);
        else rigid2d.velocity = new Vector2(rigid2d.velocity.x, 0);
    }


    IEnumerator FlipLeft() {
        for (int _i = leftSprite.Length - 1; _i >= 0; _i--) {
            spriteRenderer.sprite = leftSprite[_i];
            yield return null;
        }
    }

    IEnumerator FlipRight() {
        for (int _i = 0; _i < rightSprite.Length; _i++) {
            spriteRenderer.sprite = rightSprite[_i];
            yield return null;
        }
        yield return null;
    }

    public override void TakeDamage(int damage) {
        health -= damage;
    }
}
