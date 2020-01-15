using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boss))]
public class MovableBoss : MonoBehaviour
{
    [SerializeField]
    private Vector2 boundMin, boundMax;
    [SerializeField]
    private Vector2 drag, maxVelocity;
    private Vector2 velocity;

    [SerializeField]
    private bool idleDrift;
    private bool drifting;
    [SerializeField]
    private float waitTime;
    private Timer waitTimer;
    [SerializeField]
    private float driftTime;
    private Timer driftTimer;
    [SerializeField]
    private Vector2 driftForce;
    private Vector2 driftDirection;

    private Vector3 moveToPos;
    private bool destinationMoving;

    public Vector3 Position { get { return transform.position; } }

    private void Awake() {
        waitTimer = new Timer(waitTime);
        driftTimer = new Timer(driftTime);
    }

    private void HandleDrifting() {
        if (destinationMoving) return;

        if (drifting)
        {
            if (driftTimer.UpdateEnd) {
                driftTimer.Reset();
                drifting = false;
            } else {
                velocity += driftDirection * Time.deltaTime;
            }
        }
        else if (waitTimer.UpdateEnd) {
            drifting = true;
            waitTimer.Reset();
            // driftDirection = driftForce;// new Vector2(driftForce.x * Random.Range(-1f, 1f), driftForce.y * Random.Range(-1f, 1f));

            float x = transform.position.x;
            if (x < boundMin.x) driftDirection = driftForce;
            else if (x > boundMax.x) driftDirection = -driftForce;
            else driftDirection = Random.Range(1, 3) == 1 ? driftForce : -driftForce;
        }
    }

    private void ResolveVelocity() {
        if (transform.position.x > boundMax.x)
            if (velocity.x > 0) velocity.x = 0;
        if (transform.position.x < boundMin.x)
            if (velocity.x < 0) velocity.x = 0;
        if (transform.position.y > boundMax.y)
            if (velocity.y > 0) velocity.y = 0;
        if (transform.position.y < boundMin.y)
            if (velocity.y < 0) velocity.y = 0;
        
        velocity = new Vector2(Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x),
                               Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y));
    }

    private void Update() {
        velocity = new Vector2(Mathf.MoveTowards(velocity.x, 0, drag.x), Mathf.MoveTowards(velocity.y, 0, drag.y));
        if (idleDrift) HandleDrifting();

        ResolveVelocity();

        transform.Translate(velocity * Time.deltaTime);
    }

    public void ApplyForce(Vector2 vector) {
        waitTimer.Reset();
        driftTimer.Reset();
        drifting = false;

        velocity += vector;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((boundMin + boundMax) / 2, boundMax - boundMin);
    }
    
    
    public bool MoveTo(Vector3 position) {
        if (destinationMoving) {
            float distance = (transform.position - moveToPos).magnitude;

            if (distance < 0.05f) {
                transform.position = position;
                velocity = Vector3.zero;
                return true;
            }
            else if (distance < 1) {
                if (velocity.magnitude > 2)
                    ApplyForce(-velocity * Time.deltaTime);
            }
        }
        else {
            destinationMoving = true;
            moveToPos = position;
            ApplyForce((position - transform.position).normalized * 5);
        }
        return false;
    }
}
