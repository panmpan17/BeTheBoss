using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeaponSelectableIcon : SelectableItem
{
    private Boss.AttackType type;
    public Boss.AttackType Type { get { return type; } }
    private BossInput input;

    public void Setup(BossInput _input, Boss.AttackType _type) {
        type = _type;
        input = _input;
    }

    public override void Activate()
    {
        input.ActiveWeapon(this as SelectableItem, type);
        // if (activeEvent != null) activeEvent.Invoke();
        // AudioManager.ins.PlayerSound(AudioEnum.UIClick);
    }
}
