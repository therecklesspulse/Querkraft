using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : Attack {

    [SerializeField]
    float mass;

    bool stall = false;
    bool nailed = false;

    // Use this for initialization
    new void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (!nailed)
        {
            CalculateDynamics();
            AdjustFacing();
            if (!stall)
                CheckImpact();
            Traslate();
        }
    }

    protected override void CalculateDynamics()
    {
        base.CalculateDynamics();
        velocity += GRAVITY * mass * Time.deltaTime;
        traslation = velocity * Time.deltaTime;
    }

    override protected void AdjustFacing()
    {
        base.AdjustFacing();
        if (!stall && !velocity.Equals(Vector3.zero))
            transform.up = velocity.normalized;
        else
            transform.up = Vector3.Lerp(transform.up, Vector3.down, Time.deltaTime);
    }

    override protected void CheckImpact()
    {
        base.CheckImpact();
        LayerMask impactMask = LayerMask.GetMask("Player", "Enemy", "Level");
        Vector3 rayOrigin = transform.position;
        Vector3 rayDir = velocity.normalized;
        Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        float rayLength = bounds.extents.y - bounds.center.y + traslation.magnitude;
            
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, impactMask);
        if (hit)
        {
            if (owner.layer == LayerMask.NameToLayer("Player") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                transform.SetParent(hit.transform);
                hit.collider.gameObject.GetComponent<Enemy>().GetHurt(damage);
            }
            else if (owner.layer == LayerMask.NameToLayer("Enemy") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hit.collider.gameObject.GetComponent<Player>().GetHurt(damage);
            }

            if (hit.collider.tag == "Hard" || this.tag == "Hard")
            {
                velocity = new Vector3(-Mathf.Sign(velocity.x), 6.5f, 0f);
                mass = 1f;
                stall = true;
            }
            else {
                nailed = true;
                //transform.SetParent(hit.transform);
            }
        }
    }
}
