#pragma warning disable 649

using UnityEngine;
using ReleaseVersion.UI;

namespace ReleaseVersion {
    [RequireComponent(typeof(Boss))]
    public class BossInput : MonoBehaviour {
        private Boss boss;
        [SerializeField]
        private SelectableItem minionSelectable, machineGunSelectable, laserSelectable, bombSelectable;
        [SerializeField]
        private float aimMoveSpeed;
        private SelectableItem selectedWeapon;

        private void Awake() {
            boss = GetComponent<Boss>();
            boss.RegisterEvent(WeaponFinished, WeaponAvalible);

            selectedWeapon = minionSelectable;
            selectedWeapon.Selected = true;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                selectedWeapon.Selected = false;
                selectedWeapon = selectedWeapon.NavTop;
                selectedWeapon.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedWeapon.Selected = false;
                selectedWeapon = selectedWeapon.NavBottom;
                selectedWeapon.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space)) selectedWeapon.Activate();

            if (boss.UsingBomb || boss.UsingMachinGun || boss.UsingLaser)
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                if (Mathf.Abs(horizontal) > 0.5f) boss.keyDirection = horizontal > 0? -1: 1;
                else boss.keyDirection = 0;
            }
        }

        public void ActiveMinion() { ActiveWeapon(minionSelectable, Boss.AttackType.Minion); }
        public void ActiveBomb() { ActiveWeapon(bombSelectable, Boss.AttackType.Bomb); }
        public void ActiveMachineGun() { ActiveWeapon(machineGunSelectable, Boss.AttackType.MachineGun); }
        public void ActiveLaser() { ActiveWeapon(laserSelectable, Boss.AttackType.Laser); }

        private void ActiveWeapon(SelectableItem selectable, Boss.AttackType type) {
            if (!selectable.Disabled && !selectable.Active && boss.Idling) {
                selectable.Active = true;
                boss.NewAttack(type);
            }
        }

        private void WeaponFinished(Boss.AttackType _type)
        {
            switch (_type)
            {
                case Boss.AttackType.Laser:
                    laserSelectable.Active = false;
                    laserSelectable.Disabled = true;
                    break;
                case Boss.AttackType.Bomb:
                    bombSelectable.Active = false;
                    bombSelectable.Disabled = true;
                    break;
                case Boss.AttackType.Minion:
                    minionSelectable.Active = false;
                    minionSelectable.Disabled = true;
                    break;
                case Boss.AttackType.MachineGun:
                    machineGunSelectable.Active = false;
                    machineGunSelectable.Disabled = true;
                    break;
            }
        }

        private void WeaponAvalible(Boss.AttackType _type)
        {
            switch (_type)
            {
                case Boss.AttackType.Laser:
                    laserSelectable.Disabled = false;
                    break;
                case Boss.AttackType.Bomb:
                    bombSelectable.Disabled = false;
                    break;
                case Boss.AttackType.Minion:
                    minionSelectable.Disabled = false;
                    break;
                case Boss.AttackType.MachineGun:
                    machineGunSelectable.Disabled = false;
                    break;
            }
        }
    }
}