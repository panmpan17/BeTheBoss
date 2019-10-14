using UnityEngine;

[RequireComponent(typeof(PlayerContoller))]
public class PlayerInput : MonoBehaviour {
    private PlayerContoller contoller;

    private void Awake() {
        contoller = GetComponent<PlayerContoller>();
    }

    private void Update() {
        PlayerContoller.Movement movement = new PlayerContoller.Movement();
        if (Input.GetKey(KeyCode.RightArrow)) movement.Horizontal = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) movement.Horizontal = -1;
        if (Input.GetKey(KeyCode.UpArrow)) movement.Vertical = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) movement.Vertical = -1;

        contoller.SetNextMovement(movement);

        if (Input.GetKeyDown(KeyCode.Space)) contoller.ShootMissle();
    }
}