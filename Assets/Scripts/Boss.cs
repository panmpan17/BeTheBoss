using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Damageable
{
    public static Boss ins;

    [SerializeField]
    private SpriteRenderer healthBar;

    private LaserCanon laser;
    private MachineGun[] machineGuns;
    private BombCabinCtrl bombCabinCtrl;
    private MinionCabinCtrl minionCabinCtrl;

    private AttackType attackType = AttackType.None;
    private List<AttackType> usedAttackType;
    private enum AttackType { None, Laser, MachineGun, Minion, Bomb }

    private float laserTimeCount;
    private int laserDirection;

    void Awake() {
        SetupHealth();

        ins = this;
        usedAttackType = new List<AttackType>();

        laser = GetComponentInChildren<LaserCanon>();
        minionCabinCtrl = GetComponentInChildren<MinionCabinCtrl>();
        machineGuns = GetComponentsInChildren<MachineGun>();
        bombCabinCtrl = GetComponentInChildren<BombCabinCtrl>();
    }

    public void WeaponeFinished() {
        if (attackType != AttackType.None) attackType = AttackType.None;
        if (usedAttackType.Count >= 3) usedAttackType.RemoveAt(0);
    }

    void NewAttack(AttackType type) {
        if (usedAttackType.Contains(type)) return;

        attackType = type;
        usedAttackType.Add(type);

        switch (type) {
            case AttackType.Laser:
                laser.Activate();
                break;
            case AttackType.Minion:
                minionCabinCtrl.Activate();
                break;
            case AttackType.Bomb:
                bombCabinCtrl.Activate();
                break;
            case AttackType.MachineGun:
                for (int i = 0; i < machineGuns.Length; i++) machineGuns[i].Activate();
                break;
        }
    }

    void Update() {
        switch (attackType) {
            case AttackType.None:
                if (Input.GetKeyDown(KeyCode.S)) NewAttack(AttackType.Laser);
                else if (Input.GetKeyDown(KeyCode.M)) NewAttack(AttackType.Minion);
                else if (Input.GetKeyDown(KeyCode.N)) NewAttack(AttackType.MachineGun);
                else if (Input.GetKeyDown(KeyCode.B)) NewAttack(AttackType.Bomb);
                break;
            case AttackType.Laser:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) laser.UpdateDirection(Input.GetKeyDown(KeyCode.A)? -1: 1);
                break;
        }
    }

    void HandleDeath() {
        Debug.Log("Boss is dead");
    }

    public override void TakeDamage(int amount) {
        health -= amount;
        if (health < 0) health = 0;

        Vector2 size = healthBar.size;
        size.x = (float) health / startingHealth;
        healthBar.size = size;

        if (health == 0) HandleDeath();
    }
}
