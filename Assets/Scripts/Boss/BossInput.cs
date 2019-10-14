using UnityEngine;

[RequireComponent(typeof(Boss))]
public class BossInput : MonoBehaviour {
    private Boss boss;

    private void Awake() {
        boss = GetComponent<Boss>();
    }

    private void Update() {
        if (boss.Idling) {
            if (Input.GetKeyDown(KeyCode.S)) boss.NewAttack(Boss.AttackType.Laser);
            else if (Input.GetKeyDown(KeyCode.M)) boss.NewAttack(Boss.AttackType.Minion);
            else if (Input.GetKeyDown(KeyCode.N)) boss.NewAttack(Boss.AttackType.MachineGun);
            else if (Input.GetKeyDown(KeyCode.B)) boss.NewAttack(Boss.AttackType.Bomb);
        } else if (boss.UsingLaser) {
            if (Input.GetKeyDown(KeyCode.D)) boss.ChangeLaserDirection(-1);
            else if (Input.GetKeyDown(KeyCode.A)) boss.ChangeLaserDirection(1);
        } else if (boss.UsingMachinGun) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // TODO: make sure aim not cross scene
            if (Mathf.Abs(horizontal) > 0.5)
            {
                boss.MachineGunAim.x -= horizontal * Time.deltaTime * 3;
            }
            if (Mathf.Abs(vertical) > 0.5)
            {
                boss.MachineGunAim.y -= vertical * Time.deltaTime * 3;
            }
        }
    }
}