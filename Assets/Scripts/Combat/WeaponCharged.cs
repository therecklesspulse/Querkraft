using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCharged : Weapon {


    [System.Serializable]
    struct ChargingAttributes
    {
        public bool chargeSpeed;
        public bool chargeDamage;
        public float minSpeed;
        public float minDamage;
        public float targetCharge;
    }

    [SerializeField]
    ChargingAttributes chargingAttributes;
    
    float currentCharge = 0f;
    float chargeRatio = 0f;
    bool isCharging = false;
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (isCharging && currentCharge < chargingAttributes.targetCharge)
        {
            currentCharge += Time.deltaTime;
            chargeRatio = Mathf.Clamp01(currentCharge / chargingAttributes.targetCharge);
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

        if (chargingAttributes.chargeSpeed)
            setSpeed = chargingAttributes.minSpeed + chargeRatio * (weaponAttributes.projectileSpeed - chargingAttributes.minSpeed);
        if (chargingAttributes.chargeDamage)
            setDamage = chargingAttributes.minDamage + chargeRatio * (weaponAttributes.damage - chargingAttributes.minDamage);

        attack.SetAttack(transform.parent.gameObject, dir, setSpeed, setDamage);

        return attack;
    }
}
