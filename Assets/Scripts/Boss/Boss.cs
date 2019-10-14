using System.Collections.Generic;
using UnityEngine;
using ReleaseVersion;

public class Boss : Damageable
{
    public static Boss ins;

    public bool HasAI { get { return ai != null; } }
    public bool Idling { get { return attackType == AttackType.None; } }
    public bool UsingLaser { get { return attackType == AttackType.Laser; } }
    public bool UsingBomb { get { return attackType == AttackType.Bomb; } }
    public bool UsingMachinGun { get { return attackType == AttackType.MachineGun; } }
    public int PlayerSide { get { return (PlayerContoller.ins.transform.position.x > transform.position.x? 1: -1); } }
    public List<AttackType> UsedAttack { get { return usedAttackType; } }

    [SerializeField]
    private RectTransform healthBar;
    private float healthBarFullSize;

    private LaserCanon laser;
    private MachineGun[] machineGuns;
    private BombCabinCtrl bombCabinCtrl;
    private MinionCabinCtrl minionCabinCtrl;
    private BossAI ai;

    private AttackType attackType = AttackType.None;
    private List<AttackType> usedAttackType;
    public enum AttackType { None, Laser, MachineGun, Minion, Bomb }

    private float laserTimeCount;
    private int laserDirection;
    [System.NonSerialized]
    public Vector3 MachineGunAim = Vector3.zero;
    [SerializeField]
    private GameObject aimIndicator;

    void Awake() {
        SetupHealth();

        ins = this;
        usedAttackType = new List<AttackType>();

        laser = GetComponentInChildren<LaserCanon>();
        minionCabinCtrl = GetComponentInChildren<MinionCabinCtrl>();
        machineGuns = GetComponentsInChildren<MachineGun>();
        bombCabinCtrl = GetComponentInChildren<BombCabinCtrl>();

        healthBarFullSize = healthBar.sizeDelta.x;
    }

    void Update() {
        if (UsingMachinGun) {
            aimIndicator.transform.position = MachineGunAim;
        }
    }

    public void WeaponeFinished() {
        if (attackType == AttackType.MachineGun) aimIndicator.SetActive(false);
        if (attackType != AttackType.None) attackType = AttackType.None;
        if (usedAttackType.Count >= 3) usedAttackType.RemoveAt(0);
    }

    public void NewAttack(AttackType type) {
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
                aimIndicator.SetActive(true);
                for (int i = 0; i < machineGuns.Length; i++) machineGuns[i].Activate();
                break;
        }
    }

    public void ChangeLaserDirection(int direction) {
        laser.UpdateDirection(direction);
    }

    void HandleDeath() {
        // TODO: explosion effect
        gameObject.SetActive(false);
        GameManager.ins.BossLose();
    }

    public override void TakeDamage(int amount) {
        health -= amount;
        if (health < 0) health = 0;

        Vector2 size = healthBar.sizeDelta;
        size.x = ((float) health / startingHealth) * healthBarFullSize;
        // healthBar.size = size;
        healthBar.sizeDelta = size;

        if (health == 0) HandleDeath();
    }
}
