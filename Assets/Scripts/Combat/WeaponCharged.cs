using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCharged : Weapon {

    [SerializeField]
    bool chargeSpeed;
    [SerializeField]
    bool chargeDamage;

    [SerializeField]
    float minSpeed;
    [SerializeField]
    float minDamage;

    [SerializeField]
    float targetCharge = 1f;
    float currentCharge = 0f;
    float chargeRatio = 0f;
    bool isCharging = false;
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (isCharging && currentCharge < targetCharge)
        {
            currentCharge += Time.deltaTime;
            chargeRatio = Mathf.Clamp01(currentCharge / targetCharge);
        }
	}

    public override bool AttackDown(Vector3 dir)
    {
        bool capable = base.AttackDown(dir);
        if (capable)
        {
            isCharging = true;
        }
        return capable;
    }

    public override bool AttackUp(Vector3 dir)
    {
        bool capable = base.AttackUp(dir);
        if (capable && isCharging)
        {
            isCharging = false;
            currentCharge = 0f;
            SpawnAttack(dir);
        }
        return capable;
    }

    protected override Attack SpawnAttack(Vector3 dir)
    {
        Attack attack = base.SpawnAttack(dir);
        float setSpeed = weaponAttributes.projectileSpeed;
        float setDamage = weaponAttributes.damage;

        if (chargeSpeed)
            setSpeed = minSpeed + chargeRatio * (weaponAttributes.projectileSpeed - minSpeed);
        if (chargeDamage)
            setDamage = minDamage + chargeRatio * (weaponAttributes.damage - minDamage);

        attack.SetAttack(transform.parent.gameObject, dir, setSpeed, setDamage);

        return attack;
    }
}
