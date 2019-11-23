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
    [System.NonSerialized]
    public AttackType[] AttackTypes;
    private BossShipWeapon[] shipWeapons;

    private Timer weaponTimer;
    // private LaserCanon laser;

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

        // laser = GetComponentInChildren<LaserCanon>();

        shipWeapons = GetComponentsInChildren<BossShipWeapon>();
        List<AttackType> attackTypesList = new List<AttackType>();
        foreach (BossShipWeapon weapon in FindObjectsOfType<BossShipWeapon>()) {
            if (!attackTypesList.Contains(weapon.Type)) attackTypesList.Add(weapon.Type);
        }

        AttackTypes = attackTypesList.ToArray();
        weaponTimer = new Timer(10);
    }

    private void Start() {
        healthBar = Instantiate(Resources.Load<GameObject>("Prefab/BossHealthBar"), GameManager.ins.MainCanvas.transform).transform.GetChild(0).GetComponent<RectTransform>();
        healthBarFullSize = healthBar.sizeDelta.x;
    }

    public void RegisterEvent(AttackEvent finishedEvent, AttackEvent avalibleEvent) {
        if (finishedEvent != null) WeaponFinishedEvent += finishedEvent;
        if (avalibleEvent != null) WeaponAvalibleEvent += avalibleEvent;
    }

    void Update() {
        if (Idling) return;
        if (weaponTimer.UpdateEnd) WeaponFinished();
    }

    public void Left() {
        for (int i = 0; i < shipWeapons.Length; i++) shipWeapons[i].Left();
    }
    public void Right() {
        for (int i = 0; i < shipWeapons.Length; i++) shipWeapons[i].Right();
    }

    public void WeaponFinished() {

        bool allWeaponDeactivate = true;
        for (int i = 0; i < shipWeapons.Length; i++)
        {
            if (shipWeapons[i].Type == attackType)
            {
                allWeaponDeactivate = shipWeapons[i].Deactivate() && allWeaponDeactivate;
            }
        }

        if (allWeaponDeactivate) {
            if (WeaponFinishedEvent != null) WeaponFinishedEvent(attackType);
            attackType = AttackType.None;
            if (usedAttackType.Count > AttackTypes.Length / 2) {
                WeaponAvalibleEvent(usedAttackType[0]);
                usedAttackType.RemoveAt(0);
            }
        }
    }

    public void NewAttack(AttackType type) {
        if (usedAttackType.Contains(type)) return;

        attackType = type;
        usedAttackType.Add(type);

        for (int i = 0; i < shipWeapons.Length; i++) {
            if (shipWeapons[i].Type == type) {
                shipWeapons[i].Activate();
            }
        }
        weaponTimer.Reset();
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
