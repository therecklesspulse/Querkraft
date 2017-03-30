using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemy : Enemy {

    [SerializeField]
    float lifespan = 10f;

    const float RUN_MAX_VEL_MULT = 0.85f;

    bool die = false;

	// Use this for initialization
	new void Start () {
        base.Start();
        InitializeMovementDirection();
    }


    void InitializeMovementDirection()
    {
        PlayerList.GetNearestPlayer(transform.position, ref nearestPlayer, ref npVector);
        iAxis = npVector;
        iAxis.z = 0f;
        iAxis.Normalize();
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();

        if (awake && sleep <= 0f)
        {
            LifeSpan();
            MovementBehaviour();
        }
	}

    void LifeSpan()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f)
            die = true;

        if (die)
            Die();
    }

    protected override void MovementBehaviour()
    {
        base.MovementBehaviour();
        mov2D.AddAccelX(PlayerInput.RUN_ACCEL * Mathf.Sign(iAxis.x));
        mov2D.SetVelX(Mathf.Clamp(mov2D.GetVelocity().x, -PlayerInput.RUN_MAX_VEL * RUN_MAX_VEL_MULT, PlayerInput.RUN_MAX_VEL * RUN_MAX_VEL_MULT));
    }

    public override void SpecialCollisions(RaycastHit2D hit)
    {
        base.SpecialCollisions(hit);
        GameObject other = hit.collider.gameObject;
        if(other.layer == LayerMask.NameToLayer("Level") && Mathf.Abs(Vector3.Angle(hit.normal, Vector3.up)) > 85)
        {
            die = true;
        }
    }
}
