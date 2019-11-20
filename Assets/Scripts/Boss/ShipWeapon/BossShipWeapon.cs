using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossShipWeapon : MonoBehaviour
{
    [SerializeField]
    protected Boss.AttackType type;
    public Boss.AttackType Type { get { return type; }}

    public virtual void Activate() {
        enabled = true;
    }

    public virtual bool Deactivate() {
        enabled = false;
        return true;
    }

    public abstract void Right();
    public abstract void Left();
}
