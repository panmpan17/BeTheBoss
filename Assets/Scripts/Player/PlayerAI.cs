#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerContoller))]
public class PlayerAI : MonoBehaviour {
    private PlayerContoller controller;
    [SerializeField]
    private float reactionDelay;
    private Timer delayTimer, missleTimer;

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

    private List<Vector3> runAwayFrom, getCloseTo;
    private Vector3 result;

    private void Awake() {
        controller = GetComponent<PlayerContoller>();
        delayTimer = new Timer(reactionDelay);
        runAwayFrom = new List<Vector3>();
        getCloseTo = new List<Vector3>();
        missleTimer = new Timer(10);
    }

    private void DecideMovement() {
        runAwayFrom.Clear();
        getCloseTo.Clear();
        float x = transform.position.x, y = transform.position.y;

        if (Boss.ins.UsingLaser) {
            runAwayFrom.Add(new Vector3(Boss.ins.transform.position.x, y));
        }

        for (int _i = 0; _i < BossBomb.Pools.AliveObjects.Count; _i++) {
            Vector3 bombPos = BossBomb.Pools.AliveObjects[_i].transform.position;

            float distance = (bombPos - transform.position).sqrMagnitude;
            if (distance < 16)
            {
                runAwayFrom.Add(bombPos);
            }
            else if (bombPos.y > y && Mathf.Abs(bombPos.x - x) < 3)
            {
                getCloseTo.Add(new Vector3(bombPos.x, y));
            }
        }

        List<Weapon> bullets = WeaponPrefabPool.GetPool(WeaponType.BossBullet).AliveObjects;
        for (int _i = 0; _i < bullets.Count; _i++)
        {
            Vector3 bulletPos = bullets[_i].transform.position;

            float distance = (bulletPos - transform.position).sqrMagnitude;
            if (distance < 4)
            {
                runAwayFrom.Add(bulletPos);
            }
            else if (bulletPos.y > y && Mathf.Abs(bulletPos.x - x) < 3)
            {
                getCloseTo.Add(new Vector3(bulletPos.x, y));
            }
        }

        List<Weapon> minions = WeaponPrefabPool.GetPool(WeaponType.Minion).AliveObjects;
        for (int _i = 0; _i < minions.Count; _i++)
        {
            Vector3 minionPos = minions[_i].transform.position;

            float distance = (minionPos - transform.position).sqrMagnitude;
            if (distance < 9)
            {
                runAwayFrom.Add(minionPos);
            }
            else if (minionPos.y > y && Mathf.Abs(minionPos.x - x) < 3)
            {
                getCloseTo.Add(new Vector3(minionPos.x, y));
            }
        }

        // for (int i = 0; i < MedPack.Pools.AliveObjects.Count; i++) {
        //     Vector3 medPackPos = MedPack.Pools.AliveObjects[i].transform.position;

        //     float distance = (medPackPos - transform.position).sqrMagnitude;
        //     if (distance < 11) {
        //         getCloseTo.Add(medPackPos);
        //     }
        // }

        result = Vector3.zero;

        if (runAwayFrom.Count > 0 || getCloseTo.Count > 0) {
            for (int i = 0; i < runAwayFrom.Count; i++)
            {
                float distance = (runAwayFrom[i] - transform.position).sqrMagnitude;
                if (distance == 0) result += RandomVectorDirection();
                else result += (transform.position - runAwayFrom[i]) * Mathf.Min(0.9f / distance, 2);
            }

            for (int i = 0; i < getCloseTo.Count; i++)
            {
                float distance = (getCloseTo[i] - transform.position).sqrMagnitude;
                if (distance != 0) result += (getCloseTo[i] - transform.position) * Mathf.Min(0.4f / distance, 2);
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

        if (missleTimer.UpdateEnd) {
            missleTimer.Reset();
            controller.ShootMissle();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + result);

        if (runAwayFrom != null) {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.2f);

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

        if (getCloseTo != null) {
            Gizmos.color = new Color(0.2f, 0.2f, 1f, 0.2f);

            for (int i = 0; i < getCloseTo.Count; i++)
            {
                float distance = (getCloseTo[i] - transform.position).sqrMagnitude;
                if (distance == 0) {
                    Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);
                }
                else {
                    float mutliplier = Mathf.Min(0.4f / distance, 2);
                    Gizmos.DrawSphere(getCloseTo[i], mutliplier);
                }
            }
        }
    }
}