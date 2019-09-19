using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MachineGun : MonoBehaviour {

    [SerializeField]
    private Transform burstTransform;
    [SerializeField]
    private float rotateRangeMax, rotateRangeMin, rotateSpeed, fireRate, bulletSpeed, shootTime, weaponeTime;
    private float fireRateCount, timer;
    private bool activated, shooting;
    private int direction = 1;

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    void Update() {
        if (!activated) return;
        if (!shooting) return;

        transform.Rotate(0, 0, direction * rotateSpeed * Time.deltaTime);

        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);

        if (direction > 0 && angle > rotateRangeMax) direction *= -1;
        else if (direction < 0 && angle < rotateRangeMin) direction *= -1;

        fireRateCount += Time.deltaTime;
        if (fireRateCount >= fireRate) {
            fireRateCount = 0;
            Weapone weapone = Weapone.Spawn(WeaponeType.BossBullet);
            weapone.Set(burstTransform.position, DegreeToVector2(angle + 90) * bulletSpeed);
            weapone.transform.rotation = transform.rotation;
        }

        timer += Time.deltaTime;
        if (timer >= shootTime) shooting = false;
        if (timer >= weaponeTime) {
            activated = false;
            Boss.ins.WeaponeFinished();
        }
    }

    public void Activate() {
        activated = true;
        shooting = true;
        timer = 0;
    }
}