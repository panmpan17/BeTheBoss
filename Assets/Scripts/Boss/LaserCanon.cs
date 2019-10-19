using UnityEngine;
using System.Collections;

public class LaserCanon : MonoBehaviour {
    [SerializeField]
    private GameObject lightBeam;
    [SerializeField]
    private float time, speed, xMin, xMax;
    private Timer timer;
    private int direction;
    private bool activated;
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
        activated = false;
        lightBeam.SetActive(false);

        timer = new Timer(time);
    }

    void Update() {
        if (activated) {
            if (timer.UpdateEnd) {
                activated = false;
                lightBeam.SetActive(false);
                animator.SetTrigger("End");
                StartCoroutine(BackToCenter());
            }

            if (direction != 0) {
                Vector3 pos = transform.parent.position;
                pos.x = Mathf.Clamp(pos.x + direction * speed * Time.deltaTime, xMin, xMax);
                transform.parent.position = pos;
            }
        }
    }

    void Fire() {
        animator.ResetTrigger("Prepare");
        activated = true;
        lightBeam.SetActive(true);
    }

    IEnumerator BackToCenter() {
        Vector3 destination = transform.parent.position;
        destination.x = 0;

        while (Mathf.Abs(transform.parent.position.x - destination.x) > 0.1f) {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, destination, speed * Time.deltaTime);
            yield return false;
        }

        Boss.ins.WeaponFinished();
    }

    public void Activate() {
        timer.Reset();
        direction = 0;
        animator.SetTrigger("Prepare");
    }

    public void UpdateDirection(int _direction) {
        direction = _direction;
    }
}