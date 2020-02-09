#pragma warning disable 649
#define SHOOTING

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Audio;
using Setting.Data;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerContoller : Damageable
{
    static public PlayerContoller ins;

    private bool Left { get { return Input.GetAxis("Horizontal_Joystick") < -0.5; } }
    private bool Right { get { return Input.GetAxis("Horizontal_Joystick") > 0.5; } }
    private bool Down { get { return Input.GetAxis("Vertical_Joystick") > 0.5; } }
    private bool Up { get { return Input.GetAxis("Vertical_Joystick") < -0.5; } }

    // [SerializeField]
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
    private Image healthBar;

    // EMP stuning
    private float EMPStunSec;
    private Timer stunTimer;
    private bool stuning;

    // Life
    private int life;
    [SerializeField]
    private TextMeshProUGUI lifeText;

    private Movement movement, nextMovement;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid2d;
    private Vector3 rebirthPos;

    private int missleCount, missleMaxCount;
    private bool haveRebirthProtection;
    private Timer rebirthProtectionTimer;

    public void ApplySetting(PlayerSetting setting) {
        startingHealth = setting.StartingHealth;
        flyingSpeed = setting.FlySpeed;
        rebirthProtectionTime = setting.RebirthPortection;
        life = setting.Life;

        bulletSpeed = setting.BulletSpeed;
        fireRate = setting.BulletFireRate;

        missleCount = setting.MissleStartCount;
        missleMaxCount = setting.MissleMaxCount;
        missleSpeed = setting.MissleSpeed;

        EMPStunSec = setting.EMPStunSec;

        SetupHealth();
        rebirthProtectionTimer = new Timer(rebirthProtectionTime);
        fireRateTimer = new Timer(fireRate);
        stunTimer = new Timer(EMPStunSec);
        UpdateHealthBar();
    }

    void Awake() {
        ins = this;
        rebirthPos = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid2d = GetComponent<Rigidbody2D>();
        movement = nextMovement = new Movement();
        enabled = false;

        stunTimer = new Timer(EMPStunSec);
    }

    public void SetNextMovement(Movement movement) {
        nextMovement = movement;
    }

    public void ShootMissle() {
        if (stuning) return;

        if (missleCount > 0) {
            WeaponPrefabPool misslePool = WeaponPrefabPool.GetPool(WeaponType.PlayerMissle);
            if (misslePool.AliveObjects.Count == 0) {
                missleCount--;
                misslePool.GetFromPool().Setup(transform.position, Vector2.up * missleSpeed);
            }
        }
    }

#if SHOOTING
    void Update()
    {
        if (stuning) return;

        if (fireRateTimer.UpdateEnd) {
            fireRateTimer.Reset();
            for (int i = 0; i < burstPosition.Length; i++) {
                Weapon weapon = WeaponPrefabPool.GetFromPool(WeaponType.PlayerBullet);
                weapon.Setup(burstPosition[i].position, Vector2.up * bulletSpeed);
                AudioManager.ins.PlayerSound(AudioEnum.BulletShoot);
            }
        }
    }
#endif

    void FixedUpdate() {
        if (stuning)
        {
            if (stunTimer.UpdateEnd) stuning = false;
            return;
        }

        if (haveRebirthProtection && rebirthProtectionTimer.UpdateEnd) {
            haveRebirthProtection = false;
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }

        bool horiChange, VertiChange;
        if (movement.Different(nextMovement, out horiChange, out VertiChange)) {
            if (horiChange) {
                StopCoroutine("FlipRight");
                StopCoroutine("FlipLeft");
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
        healthBar.fillAmount = (float)health / startingHealth;
        lifeText.text = life.ToString();
    }

    void HandleDeath() {
        // TODO: destroy explosion effect
        if (life == 0) {
            gameObject.SetActive(false);
            GameManager.ins.PlayerLose();
        } else {
            life -= 1;
            lifeText.text = life.ToString();

            HandleRebirth();
        }
    }

    void HandleRebirth() {
        StopAllCoroutines();
        transform.position = rebirthPos;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        haveRebirthProtection = true;
        rebirthProtectionTimer.Reset();

        stuning = false;
        stunTimer.Reset();

        health = startingHealth;
        UpdateHealthBar();
    }

    public override bool TakeDamage(int damage, GameObject other) {
        if (haveRebirthProtection) return false;

        if (other.CompareTag("EMP")) {
            stuning = true;
            stunTimer.Reset();
        }

        health -= damage;
        if (health < 0) {
            HandleDeath();
            return true;
        }
        UpdateHealthBar();

        StopCoroutine("HurtBlink");
        StartCoroutine(HurtBlink());
        return false;
    }

    IEnumerator HurtBlink(float interval = 0.1f, float times = 3) {
        WaitForSeconds wait = new WaitForSeconds(interval);
        bool goDark = true;

        for (int i = 0; i < times * 2; i++) {
            spriteRenderer.color = goDark? Color.grey: Color.white;
            goDark = !goDark;
            yield return wait;
        }
    }

    #region  Reward
    public void AddHealth(int healthReward) {
        health += healthReward;
        if (health >= startingHealth) health = startingHealth;
        UpdateHealthBar();
    }

    public void AddMissile(int amount) {
        missleCount += amount;
        if (missleCount > missleMaxCount) missleCount = missleMaxCount;
    }
    #endregion

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
