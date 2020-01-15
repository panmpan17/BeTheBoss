#pragma warning disable 649

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Boss))]
public class BossInput : MonoBehaviour {
    static private GameObject _iconPrefab;
    static private GameObject iconPrefab { get {
        if (_iconPrefab == null) _iconPrefab = Resources.Load<GameObject>("Prefab/BossInputIcon");
        return _iconPrefab;
    } }

    private Boss boss;
    [SerializeField]
    private ShipWeaponSelectableIcon[] shipWeaponSelectables;
    private SelectableItem selected;

    private void Awake() {
        boss = GetComponent<Boss>();
        boss.RegisterEvent(WeaponFinished, WeaponAvalible);
        SpawnShipWeaponIcon();
    }

    void SpawnShipWeaponIcon() {
        Canvas canvas = GameManager.ins.GetCopyOfCanvas("ShipWeaponIcons");

        float y = 0;
        shipWeaponSelectables = new ShipWeaponSelectableIcon[boss.AttackTypes.Length];
        for (int i = 0; i < boss.AttackTypes.Length; i++) {
            Boss.AttackType _type = boss.AttackTypes[i];

            RectTransform newIconT = Instantiate(iconPrefab, canvas.transform).GetComponent<RectTransform>();
            Vector3 pos = newIconT.anchoredPosition;
            pos.y = y;
            newIconT.anchoredPosition = pos;
            y += 80;

            newIconT.GetComponentInChildren<TextMeshProUGUI>().text = (boss.AttackTypes.Length - i).ToString();
            newIconT.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Icon/" + _type.ToString());

            ShipWeaponSelectableIcon current = shipWeaponSelectables[i] = newIconT.GetComponent<ShipWeaponSelectableIcon>();
            current.Setup(this, _type);

            if (i > 0) {
                shipWeaponSelectables[i - 1].NavTop = shipWeaponSelectables[i];
                shipWeaponSelectables[i].NavBottom = shipWeaponSelectables[i - 1];
            }
        }

        selected = shipWeaponSelectables[shipWeaponSelectables.Length - 1];
        selected.Selected = true;
    }

    private void Update() {
        if (GameManager.ins.GamePaused) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selected.NavTop == null) return;
            selected.Selected = false;
            selected = selected.NavTop;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selected.NavBottom == null) return;
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            if (!selected.Disabled && !selected.Active && boss.Idling) selected.Activate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) PressSortKey(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) PressSortKey(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) PressSortKey(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) PressSortKey(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) PressSortKey(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) PressSortKey(6);

        if (!boss.Idling)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > 0.5f) boss.Left();
            else if (horizontal < -0.5f) boss.Right();
            // if (Mathf.Abs(horizontal) > 0.5f) {} // boss.keyDirection = horizontal > 0? -1: 1;
        }
    }

    private void PressSortKey(int pressedNums) {
        if (pressedNums <= shipWeaponSelectables.Length) {
            shipWeaponSelectables[shipWeaponSelectables.Length - pressedNums].Activate();
        }
    }

    public void ActiveWeapon(SelectableItem selectable, Boss.AttackType type) {
        if (!selectable.Disabled && !selectable.Active && boss.Idling) {
            selectable.Active = true;
            boss.NewAttack(type);
        }
    }

    private void WeaponFinished(Boss.AttackType _type)
    {
        for (int i = 0; i < shipWeaponSelectables.Length; i++)
        {
            if (shipWeaponSelectables[i].Type == _type)
            {
                shipWeaponSelectables[i].Active = false;
                shipWeaponSelectables[i].Disabled = true;
            }
        }
    }

    private void WeaponAvalible(Boss.AttackType _type)
    {
        for (int i = 0; i < shipWeaponSelectables.Length; i++)
        {
            if (shipWeaponSelectables[i].Type == _type) shipWeaponSelectables[i].Disabled = false;
        }
    }
}