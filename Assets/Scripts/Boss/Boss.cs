#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;


public class Boss : Damageable
{
    public static Boss ins;

    public bool Idling { get { return attackType == AttackType.None; } }
    public bool UsingLaser { get { return attackType == AttackType.Laser; } }
    public bool UsingBomb { get { return attackType == AttackType.Bomb; } }
    public bool UsingMachinGun { get { return attackType == AttackType.MachineGun; } }
    public int PlayerSide { get { return (PlayerContoller.ins.transform.position.x > transform.position.x? 1: -1); } }
    public List<AttackType> UsedAttack { get { return usedAttackType; } }

    private float healthBarFullSize;

    [SerializeField]
    private float machineGunTime, bombTime, minionTime;
    private Timer weaponTimer;
    private LaserCanon laser;
    private MachineGun[] machineGuns;
    private BombCabinDoor[] bombCabinDoors;
    private MinionCabinDoor[] minionCabinDoors;

    private AttackType attackType = AttackType.None;
    private List<AttackType> usedAttackType;
    public enum AttackType { None, Laser, MachineGun, Minion, Bomb }

    [System.NonSerialized]
    public int keyDirection;

    public delegate void AttackEvent(AttackType type);
    private AttackEvent WeaponFinishedEvent = null, WeaponAvalibleEvent = null;
    private RectTransform healthBar;

    void Awake() {
        SetupHealth();

        ins = this;
        usedAttackType = new List<AttackType>();

        laser = GetComponentInChildren<LaserCanon>();
        minionCabinDoors = GetComponentsInChildren<MinionCabinDoor>();
        machineGuns = GetComponentsInChildren<MachineGun>();
        bombCabinDoors = GetComponentsInChildren<BombCabinDoor>();

        healthBar = Instantiate(Resources.Load<GameObject>("Prefab/BossHealthBar"), FindObjectOfType<Canvas>().transform).transform.GetChild(0).GetComponent<RectTransform>();
        healthBarFullSize = healthBar.sizeDelta.x;
    }

    public void RegisterEvent(AttackEvent finishedEvent, AttackEvent avalibleEvent) {
        if (finishedEvent != null) WeaponFinishedEvent += finishedEvent;
        if (avalibleEvent != null) WeaponAvalibleEvent += avalibleEvent;
    }

    void Update() {
        if (Idling) return;

        if (UsingMachinGun) {
            for (int i = 0; i < machineGuns.Length; i++) {
                if (machineGuns[i].IsAimMode) machineGuns[i].Rotate(keyDirection);
            }
        } else if (UsingBomb) {
            for (int i = 0; i < bombCabinDoors.Length; i++) bombCabinDoors[i].Rotate(keyDirection);
        } else if (UsingLaser) {
            laser.UpdateDirection(keyDirection);
        }

        if (!UsingLaser && weaponTimer.UpdateEnd) WeaponFinished();
    }

    public void WeaponFinished() {
        if (WeaponFinishedEvent != null) WeaponFinishedEvent(attackType);
        attackType = AttackType.None;
        if (usedAttackType.Count >= 3) {
            WeaponAvalibleEvent(usedAttackType[0]);
            usedAttackType.RemoveAt(0);
        }
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
                for (int i = 0; i < minionCabinDoors.Length; i++) minionCabinDoors[i].Activate();
                weaponTimer = new Timer(minionTime);
                break;
            case AttackType.Bomb:
                for (int i = 0; i < bombCabinDoors.Length; i++) bombCabinDoors[i].Activate();
                weaponTimer = new Timer(bombTime);
                break;
            case AttackType.MachineGun:
                for (int i = 0; i < machineGuns.Length; i++) machineGuns[i].Activate();
                weaponTimer = new Timer(machineGunTime);
                break;
        }
    }

    void HandleDeath() {
        // TODO: explosion effect
        gameObject.SetActive(false);
        GameManager.ins.BossLose();
    }

    public override bool TakeDamage(int amount) {
        health -= amount;
        if (health < 0) health = 0;

        Vector2 size = healthBar.sizeDelta;
        size.x = ((float) health / startingHealth) * healthBarFullSize;
        healthBar.sizeDelta = size;

        if (health == 0) {
            HandleDeath();
            return true;
        }
        return false;
    }
}
