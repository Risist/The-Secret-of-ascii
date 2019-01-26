using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;

/*
 * Base class for all weapons
 */
public abstract class WeaponBase
{
    public static WeaponBase GetNewWeapon(int id)
    {
        if (id == 0)
            return new WeaponDagger();
        else if (id == 1)
            return new WeaponSpear();
        else if (id == 2)
            return new WeaponBow();
        else if (id == 3)
            return new WeaponSwordShield();

        return null;
    }

    /// sets up CharacterStateController data
    public abstract void InitWeapon(CharacterStateController controller);
    /// cleans up CharacterStateController data
    public virtual void CleanUpWeapon(CharacterStateController controller)
    {
        controller.ClearStates();
    }
}
