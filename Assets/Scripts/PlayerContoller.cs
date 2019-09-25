// #define SHOOTING

using System.Collections;
using UnityEngine;
using TMPro;

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
    static public PlayerContoller ins;

    // private bool Left { get { return Input.GetKey(KeyCode.LeftArrow); } }
    // private bool Right { get { return Input.GetKey(KeyCode.RightArrow); } }
    // private bool Down { get { return Input.GetKey(KeyCode.DownArrow); } }
    // private bool Up { get { return Input.GetKey(KeyCode.UpArrow); } }
    private bool Left { get { return Input.GetAxis("Horizontal_Joystick") < -0.5; } }
    private bool Right { get { return Input.GetAxis("Horizontal_Joystick") > 0.5; } }
    private bool Down { get { return Input.GetAxis("Vertical_Joystick") > 0.5; } }
    private bool Up { get { return Input.GetAxis("Vertical_Joystick") < -0.5; } }

    [SerializeField]
    private float speed, fireRate, bulletSpeed, missleSpeed, rebirthProtectionTime;
    private Timer fireRateTimer;
    [SerializeField]
    private Sprite[] leftSprite, rightSprite;
    [SerializeField]
    private Sprite idleSprite;
    [SerializeField]
    private Transform[] burstPosition;
    [SerializeField]
    private Color fullHealthColor, emptyHealthColor;
    [SerializeField]
    private SpriteRenderer healthBar;

    [SerializeField]
    private int life;
    [SerializeField]
    private TextMeshProUGUI lifeText;

    private State state;
    private enum State { Left, Idle, Right }

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid2d;
    private Vector3 rebirthPos;
    private bool usingMissle;

    private bool haveRebirthProtection;
    private Timer rebirthProtectionTimer;

    void Awake() {
        ins = this;
        rebirthPos = transform.position;
        SetupHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid2d = GetComponent<Rigidbody2D>();

        rebirthProtectionTimer = new Timer(rebirthProtectionTime);
        fireRateTimer = new Timer(fireRate);
    }

    void Update()
    {
#if SHOOTING
        if (fireRateTimer.UpdateEnd) {
            fireRateTimer.Reset();
            for (int i = 0; i < burstPosition.Length; i++) Weapone.Spawn(WeaponeType.PlayerBullet).Set(burstPosition[i].position, Vector2.up * bulletSpeed);
        }
#endif

        if (Input.GetKeyDown(KeyCode.Space) && !usingMissle) {
            PlayerMissle missle = PlayerMissle.Get();
            missle.Setup(transform.position, Vector2.up * missleSpeed);
            usingMissle = true;
        }
    }

    public void MissleEnd() {
        usingMissle = false;
    }

    void FixedUpdate() {
        if (haveRebirthProtection && rebirthProtectionTimer.UpdateEnd) {
            haveRebirthProtection = false;
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }

        // State newState = Left ? State.Left: (Right? State.Right: State.Idle);
        State newState = State.Idle;

        if (state != newState) {
            StopAllCoroutines();
            state = newState;
            switch (newState) {
                case State.Left:
                    StartCoroutine(FlipLeft());
                    break;
                case State.Right:
                    StartCoroutine(FlipRight());
                    break;
                case State.Idle:
                    rigid2d.velocity = Vector2.zero;
                    spriteRenderer.sprite = idleSprite;
                    break;
            }
        }

        switch (state) {
            case State.Left:
                rigid2d.velocity = new Vector2(-speed, 0);
                break;
            case State.Right:
                rigid2d.velocity = new Vector2(speed, 0);
                break;
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

    void HandleDeath() {
        life -= 1;
        lifeText.text = life.ToString();

        // Rebirth
        transform.position = rebirthPos;
        Color color = spriteRenderer.color;
        color.a = 0.5f;
        spriteRenderer.color = color;

        health = startingHealth;
        healthBar.color = fullHealthColor;
        Vector2 size = healthBar.size;
        size.x = 1;
        healthBar.size = size;

        haveRebirthProtection = true;
        rebirthProtectionTimer.Reset();
    }

    public override void TakeDamage(int damage) {
        if (haveRebirthProtection) return;

        health -= damage;
        if (health < 0) health = 0;

        healthBar.color = Color.Lerp(fullHealthColor, emptyHealthColor, (float) health / startingHealth);
        Vector2 size = healthBar.size;
        size.x = (float) health / startingHealth;
        healthBar.size = size;

        if (health == 0) HandleDeath();
    }
}
