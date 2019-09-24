using UnityEngine;
using System.Collections;

public class LaserCanon : MonoBehaviour {
    [SerializeField]
    private GameObject lightBeam;
    [SerializeField]
    private float time, speed, xMin, xMax;
    private float timeCount;
    private int direction;
    private bool activated;
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
        activated = false;
        lightBeam.SetActive(false);
    }

    void Update() {
        if (activated) {
            timeCount += Time.deltaTime;
            if (timeCount >= time) {
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

        Boss.ins.WeaponeFinished();
    }

    public void Activate() {
        timeCount = 0;
        direction = 0;
        animator.SetTrigger("Prepare");
    }

    public void UpdateDirection(int _direction) {
        if (direction == 0) direction = _direction;
    }
}