using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossShipWeapon : MonoBehaviour
{
    public virtual void Activate() {
        enabled = true;
    }

    public virtual void Deactivate() {
        enabled = false;
    }
}
