using UnityEngine;
using ReleaseVersion.UI;

namespace ReleaseVersion {
    [RequireComponent(typeof(Boss))]
    public class BossInput : MonoBehaviour {
        private Boss boss;
        [SerializeField]
        private SelectableItem minionSelectable, machineGUnSelectable, laserSelectable, bombSelectable;
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
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                selectedWeapon.Selected = false;
                selectedWeapon = selectedWeapon.NavTop;
                selectedWeapon.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedWeapon.Selected = false;
                selectedWeapon = selectedWeapon.NavBottom;
                selectedWeapon.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space)) {
                switch (selectedWeapon.Arg) {
                    case "minion":
                        ActiveWeapon(selectedWeapon, Boss.AttackType.Minion);
                        break;
                    case "machineGun":
                        ActiveWeapon(selectedWeapon, Boss.AttackType.MachineGun);
                        break;
                    case "bomb":
                        ActiveWeapon(selectedWeapon, Boss.AttackType.Bomb);
                        break;
                    case "laser":
                        ActiveWeapon(selectedWeapon, Boss.AttackType.Laser);
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) ActiveWeapon(minionSelectable, Boss.AttackType.Minion);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ActiveWeapon(bombSelectable, Boss.AttackType.Bomb);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ActiveWeapon(machineGUnSelectable, Boss.AttackType.MachineGun);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) ActiveWeapon(laserSelectable, Boss.AttackType.Laser);

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(horizontal) > 0.5f || Mathf.Abs(vertical) > 0.5f) {
                boss.MachineGunAim -= new Vector3(horizontal, vertical) * aimMoveSpeed * Time.deltaTime;
            }
        }

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
                    machineGUnSelectable.Active = false;
                    machineGUnSelectable.Disabled = true;
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
                    machineGUnSelectable.Disabled = false;
                    break;
            }
        }
    }
}