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
        private SelectableItem selectedWeapone;

        private void Awake() {
            boss = GetComponent<Boss>();
            boss.RegisterEvent(WeaponeFinished, WeaponeAvalible);

            selectedWeapone = minionSelectable;
            selectedWeapone.Selected = true;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                selectedWeapone.Selected = false;
                selectedWeapone = selectedWeapone.NavTop;
                selectedWeapone.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedWeapone.Selected = false;
                selectedWeapone = selectedWeapone.NavBottom;
                selectedWeapone.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space)) {
                switch (selectedWeapone.Arg) {
                    case "minion":
                        ActiveWeapone(selectedWeapone, Boss.AttackType.Minion);
                        break;
                    case "machineGun":
                        ActiveWeapone(selectedWeapone, Boss.AttackType.MachineGun);
                        break;
                    case "bomb":
                        ActiveWeapone(selectedWeapone, Boss.AttackType.Bomb);
                        break;
                    case "laser":
                        ActiveWeapone(selectedWeapone, Boss.AttackType.Laser);
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) ActiveWeapone(minionSelectable, Boss.AttackType.Minion);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ActiveWeapone(bombSelectable, Boss.AttackType.Bomb);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ActiveWeapone(machineGUnSelectable, Boss.AttackType.MachineGun);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) ActiveWeapone(laserSelectable, Boss.AttackType.Laser);

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(horizontal) > 0.5f || Mathf.Abs(vertical) > 0.5f) {
                boss.MachineGunAim -= new Vector3(horizontal, vertical) * aimMoveSpeed * Time.deltaTime;
            }
        }

        private void ActiveWeapone(SelectableItem selectable, Boss.AttackType type) {
            if (!selectable.Disabled && !selectable.Active && boss.Idling) {
                selectable.Active = true;
                boss.NewAttack(type);
            }
        }

        private void WeaponeFinished(Boss.AttackType _type)
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

        private void WeaponeAvalible(Boss.AttackType _type)
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