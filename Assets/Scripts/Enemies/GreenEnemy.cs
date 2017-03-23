﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemy : Enemy {

    const float DISTANCE_FOR_JUMP = 2f;
    const float DISTANCE_FOR_JUMP_MARGIN = 0.5f;
    const float SHOOTING_RANGE = 0.5f;
    const string ATTACK_NAME = "Rock";
    const float ATTACK_COOLDOWN = 0.5f;

    float attackCooldown = 0f;

	// Use this for initialization
	new void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (sleep <= 0f)
        {
            MovementBehaviour();
            if (attackCooldown > 0f)
                attackCooldown -= Time.deltaTime;
            else {
                ShootingBehaviour();
            }
            
        }
	}


    protected override void MovementBehaviour()
    {
        base.MovementBehaviour();
        if (reflexesTimer <= 0)
        {
            iAxis = (nearestPlayer.transform.position - transform.position);
            iAxis.z = 0f;
            iAxis.Normalize();
        }
        mov2D.AddAccelX(CharacterInput.RUN_ACCEL * Mathf.Sign(iAxis.x));
        mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x, -CharacterInput.RUN_MAX_VEL, CharacterInput.RUN_MAX_VEL));
        if (mov2D.Grounded && Mathf.Abs(npVector.x) > DISTANCE_FOR_JUMP - DISTANCE_FOR_JUMP_MARGIN && Mathf.Abs(npVector.x) < DISTANCE_FOR_JUMP + DISTANCE_FOR_JUMP_MARGIN)
            mov2D.SetVel((iAxis + 1.5f*Vector3.up).normalized * CharacterInput.JUMP_FORCE);
    }

    void ShootingBehaviour()
    {
        if(Mathf.Abs(transform.position.x-nearestPlayer.transform.position.x) < SHOOTING_RANGE && transform.position.y > nearestPlayer.transform.position.y)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        attackCooldown = ATTACK_COOLDOWN;
        Attack attack = Instantiate(Resources.Load<GameObject>(ATTACKS_PATH + ATTACK_NAME), transform.position, Quaternion.identity).GetComponent<Attack>();
        attack.SetAttack(gameObject, Vector3.up, 1f, 1);
    }
}