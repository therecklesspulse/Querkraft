using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    [SerializeField]
    PlayerInput playerInput;

	// Use this for initialization
	new void Start () {
        base.Start();

        if (!playerInput)
            playerInput = GetComponent<PlayerInput>();

        Debug.Log("Health: " + health);
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	}

    override public void GetHurt(int damage)
    {
        base.GetHurt(damage);
        graceTime = GRACE_TIME;
        Debug.Log("Health: " + health);
        if (health <= 0) {
            Debug.Log("YOU LOSE");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    override public void SpecialCollisions(RaycastHit2D hit)
    {
        base.SpecialCollisions(hit);
        GameObject other = hit.collider.gameObject;
        if (!IsGraced())
        {
            if (IsObstacle(other))
            {
                Obstacle obstacle = other.GetComponent<Obstacle>();
                //obstacle.SpecialCollisions(gameObject);
                GetHurt(obstacle.Damage);
                BePushed(GetPushVector(other.transform.position.x, transform.position.x), obstacle.PushForce);
            }
            else if (IsEnemy(other))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.SpecialCollisions(hit);
            }
        }
    }

    public void BePushed(Vector3 dir, float force)
    {
        mov2D.AddOverridingVel(dir * force);
    }
}
