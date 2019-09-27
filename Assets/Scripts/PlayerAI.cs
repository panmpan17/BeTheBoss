using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerContoller))]
public class PlayerAI : MonoBehaviour {
    private PlayerContoller controller;
    [SerializeField]
    private float reactionDelay;
    private Timer delayTimer;

    static private Vector3 RandomVectorDirection() {
        switch(Random.Range(0, 8)) {
            case 0:
                return new Vector3(5, -5);
            case 1:
                return new Vector3(5, 1);
            case 2:
                return new Vector3(5, 5);
            case 3:
                return new Vector3(1, 5);
            case 4:
                return new Vector3(1, -5);
            case 5:
                return new Vector3(-5, -5);
            case 6:
                return new Vector3(-5, 1);
            default:
                return new Vector3(-5, 5);
        }
    }

    private List<Vector3> runAwayFrom;
    private Vector3 result;

    private void Awake() {
        controller = GetComponent<PlayerContoller>();
        delayTimer = new Timer(reactionDelay);
        runAwayFrom = new List<Vector3>();
    }

    private void DecideMovement() {
        runAwayFrom.Clear();
        float x = transform.position.x, y = transform.position.y;

        if (Boss.ins.UsingLaser) {
            runAwayFrom.Add(new Vector3(Boss.ins.transform.position.x, y));
        }

        for (int _i = 0; _i < BossBomb.AliveObjects.Count; _i++) {
            Vector3 bombPos = BossBomb.AliveObjects[_i].transform.position;

            float distance = (bombPos - transform.position).sqrMagnitude;
            if (distance < 16) {
                runAwayFrom.Add(bombPos);
            }
        }

        List<Weapone> bullets = Weapone.WeaponePrefabPool.GetPool(WeaponeType.BossBullet).AliveObjects;
        for (int _i = 0; _i < bullets.Count; _i++)
        {
            Vector3 bulletPos = bullets[_i].transform.position;

            float distance = (bulletPos - transform.position).sqrMagnitude;
            if (distance < 4)
            {
                runAwayFrom.Add(bulletPos);
            }
        }

        List<Weapone> minions = Weapone.WeaponePrefabPool.GetPool(WeaponeType.Minion).AliveObjects;
        for (int _i = 0; _i < minions.Count; _i++)
        {
            Vector3 minionPos = minions[_i].transform.position;

            float distance = (minionPos - transform.position).sqrMagnitude;
            if (distance < 9)
            {
                runAwayFrom.Add(minionPos);
            }
        }

        result = Vector3.zero;

        if (runAwayFrom.Count > 0) {
            for (int i = 0; i < runAwayFrom.Count; i++)
            {
                float distance = (runAwayFrom[i] - transform.position).sqrMagnitude;
                if (distance == 0) {
                    result += RandomVectorDirection();
                }
                else {
                    Vector3 vec = transform.position - runAwayFrom[i];
                    float mutliplier = Mathf.Min(0.9f / distance, 2);
                    result += (vec) * mutliplier;

                }
            }
        } else {
            result.x = Boss.ins.transform.position.x - x;
        }

        controller.SetNextMovement(new PlayerContoller.Movement(result));
    }

    private void Update() {
        if (delayTimer.UpdateEnd) {
            delayTimer.Reset();

            DecideMovement();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + result);
        Gizmos.DrawLine(transform.position, transform.position + result);
        Gizmos.DrawLine(transform.position, transform.position + result);

        if (runAwayFrom != null) {
            Gizmos.color = Color.red;
            for (int i = 0; i < runAwayFrom.Count; i++)
            {
                float distance = (runAwayFrom[i] - transform.position).sqrMagnitude;
                if (distance == 0) {
                    Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);
                }
                else {
                    float mutliplier = Mathf.Min(0.9f / distance, 2);
                    Gizmos.DrawSphere(runAwayFrom[i], mutliplier);
                }
            }
        }
    }
}