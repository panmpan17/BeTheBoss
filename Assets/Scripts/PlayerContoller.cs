#define SHOOTING

using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerContoller : Damageable
{
    static public PlayerContoller ins;

    private bool Left { get { return Input.GetAxis("Horizontal_Joystick") < -0.5; } }
    private bool Right { get { return Input.GetAxis("Horizontal_Joystick") > 0.5; } }
    private bool Down { get { return Input.GetAxis("Vertical_Joystick") > 0.5; } }
    private bool Up { get { return Input.GetAxis("Vertical_Joystick") < -0.5; } }

    [SerializeField]
    private float flyingSpeed, fireRate, bulletSpeed, missleSpeed, rebirthProtectionTime;
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

    private Movement movement, nextMovement;

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

        movement = nextMovement = new Movement();
    }

    public void SetNextMovement(Movement movement) {
        nextMovement = movement;
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

        bool horiChange, VertiChange;
        if (movement.Different(nextMovement, out horiChange, out VertiChange)) {
            if (horiChange) {
                StopAllCoroutines();
                switch (nextMovement.Horizontal) {
                    case 0:
                        rigid2d.velocity = Vector2.zero;
                        spriteRenderer.sprite = idleSprite;
                        break;
                    case 1:
                        StartCoroutine(FlipRight());
                        break;
                    case -1:
                        StartCoroutine(FlipLeft());
                        break;
                }
            }

            movement = nextMovement;
        }

        rigid2d.velocity = movement.GetVelocity(flyingSpeed);
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

    void UpdateHealthBar() {
        healthBar.color = Color.Lerp(emptyHealthColor, fullHealthColor, (float)health / startingHealth);
        Vector2 size = healthBar.size;
        size.x = (float)health / startingHealth;
        healthBar.size = size;
    }

    void HandleDeath() {
        life -= 1;
        lifeText.text = life.ToString();

        HandleRebirth();
    }

    void HandleRebirth() {
        transform.position = rebirthPos;
        Color color = spriteRenderer.color;
        color.a = 0.5f;
        spriteRenderer.color = color;

        haveRebirthProtection = true;
        rebirthProtectionTimer.Reset();

        health = startingHealth;
        UpdateHealthBar();
    }

    public override void TakeDamage(int damage) {
        if (haveRebirthProtection) return;

        health -= damage;
        if (health < 0) HandleDeath();
        else UpdateHealthBar();
    }

    public class Movement
    {
        private int horizontal;
        private int vertical;
        public int Horizontal { get { return horizontal; } set { horizontal = value; } }
        public int Vertical { get { return vertical; } set { vertical = value; } }

        public Movement()
        {
            horizontal = 0;
            vertical = 0;
        }
        public Movement(Vector2 vec)
        {
            horizontal = Mathf.Abs(vec.x) < 0.1f ? 0 : (vec.x > 0 ? 1: -1);
            vertical = Mathf.Abs(vec.y) < 0.1f ? 0 : (vec.y > 0 ? 1 : -1);
        }
        public Movement(int _horizontal, int _vertical)
        {
            horizontal = _horizontal;
            vertical = _vertical;
        }

        public override string ToString() {
            return string.Format("Movement({0}, {1})", horizontal, vertical);
        }

        public bool Different(Movement other, out bool horizontalChange, out bool verticalChange)
        {
            if (horizontal != other.horizontal) horizontalChange = true;
            else horizontalChange = false;
            if (vertical != other.vertical) verticalChange = true;
            else verticalChange = false;

            return horizontalChange || verticalChange;
        }

        public Vector2 GetVelocity(float speed)
        {
            return new Vector2(horizontal * speed, vertical * speed);
        }
    }
}
