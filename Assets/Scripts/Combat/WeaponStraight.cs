using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStraight : Weapon {

    const float STRAIGHT_MAX_CHARGE = 1f;

    public override bool AttackDown(Vector3 dir)
    {
        bool capable = base.AttackDown(dir);
        if (capable)
        {
            SpawnAttack(dir);
        }
        return capable;
    }

    protected override Attack SpawnAttack(Vector3 dir)
    {
        Attack attack = base.SpawnAttack(dir);
        attack.SetAttack(transform.parent.gameObject, dir, weaponAttributes.projectileSpeed, weaponAttributes.damage);
        return attack;
    }
}
