using UnityEngine;
// using UnityEngine.SocialPlatforms;
using ReleaseVersion;

public class MachineGun : MonoBehaviour {

    [SerializeField]
    private Transform burstTransform;
    [SerializeField]
    private float rotateRangeMax, rotateRangeMin, rotateSpeed, fireRate, bulletSpeed, shootTime, weaponeTime;
    [SerializeField]
    private bool isAimMode;
    private Timer fireRateTimer;
    private float timer;
    private bool activated, shooting;
    private int direction = 1;
    private bool weaponeFinishedCalled;

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    void Awake() {
        fireRateTimer = new Timer(fireRate);
    }

    void Update() {
        if (!activated) return;
        if (shooting) {
            float angle;
            if (isAimMode) {
                Vector3 vectorToTarget = Boss.ins.MachineGunAim - transform.position;
                angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
            } else {
                transform.Rotate(0, 0, direction * rotateSpeed * Time.deltaTime);

                Vector3 axis;
                transform.rotation.ToAngleAxis(out angle, out axis);

                if (direction > 0 && angle > rotateRangeMax) direction *= -1;
                else if (direction < 0 && angle < rotateRangeMin) direction *= -1;
                angle += 90;
            }

            if (fireRateTimer.UpdateEnd) {
                fireRateTimer.Reset();
                WeaponePrefabPool.GetPool(WeaponeType.BossBullet).GetFromPool().Setup(burstTransform.position, DegreeToVector2(angle) * bulletSpeed, transform.rotation);
            }
        }

        timer += Time.deltaTime;
        if (timer >= shootTime ) {
            shooting = false;

            if (weaponeFinishedCalled) activated = false;
        }
        if (timer >= weaponeTime && !weaponeFinishedCalled) {
            weaponeFinishedCalled = true;
            Boss.ins.WeaponeFinished();

            if (!shooting) activated = false;
        }
    }

    public void Activate() {
        activated = true;
        shooting = true;
        weaponeFinishedCalled = false;
        timer = 0;
    }
}