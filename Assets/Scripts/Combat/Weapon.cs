using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField]
    protected string attackPrefabName;

    [System.Serializable]
    protected struct WeaponAttributes
    {
        public string name;
        public float weight;
        public float damage;
        public float projectileSpeed;
        public float cooldown;
        public bool isMelee;
    }
    public bool IsMelee
    {
        get
        { return weaponAttributes.isMelee; }
    }

    [SerializeField]
    protected WeaponAttributes weaponAttributes;
    protected float currentCooldown;


    void Start()
    {

    }

    protected void Update()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;
    }


    public virtual bool AttackDown(Vector3 dir)
    {
        bool capable = currentCooldown <= 0f;
        return capable;
    }

    public virtual bool AttackUp(Vector3 dir)
    {
        bool capable = currentCooldown <= 0f;
        return capable;
    }

    protected virtual Attack SpawnAttack(Vector3 dir)
    {
        currentCooldown = weaponAttributes.cooldown;
        Attack attack = Instantiate(Resources.Load<GameObject>("Prefabs/Attacks/" + attackPrefabName), transform.position, Quaternion.identity).GetComponent<Attack>();
        if(weaponAttributes.isMelee)
            attack.transform.parent = transform;
        return attack;
    }
}
