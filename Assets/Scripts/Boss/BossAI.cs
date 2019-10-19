using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boss))]
public class BossAI : MonoBehaviour
{
    private Boss boss;

    private void Awake() {
        boss = GetComponent<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.Idling) {
            List<Boss.AttackType> WeaponTypes = new List<Boss.AttackType> {
                Boss.AttackType.Laser,
                Boss.AttackType.MachineGun,
                Boss.AttackType.Bomb,
                Boss.AttackType.Minion,
            };
            foreach (Boss.AttackType item in boss.UsedAttack) {
                WeaponTypes.Remove(item);
            }

            boss.NewAttack(WeaponTypes[Random.Range(0, WeaponTypes.Count)]);
        } else if (boss.UsingLaser) {
            boss.ChangeLaserDirection(boss.PlayerSide);
        } else if (boss.UsingMachinGun)
        {
            boss.MachineGunAim = PlayerContoller.ins.transform.position;
        }
    }
}
