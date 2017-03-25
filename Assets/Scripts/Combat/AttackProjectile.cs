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
        Vector3 rayOrigin = transform.position;
        Vector3 rayDir = velocity.normalized;
        float rayLength = bounds.extents.y - bounds.center.y + traslation.magnitude;
            
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, impactMask);
        if (hit)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                HitSomeone(hit);

                if (owner.layer == LayerMask.NameToLayer("Player") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    transform.SetParent(hit.transform);
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

    override protected void InitializeImpactParams()
    {
        base.InitializeImpactParams();
        string characterLayer = GetImpactLayerByOwnerLayer();
        impactMask = LayerMask.GetMask(characterLayer, "Level");
    }
}
