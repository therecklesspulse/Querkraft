using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlash : Attack {

	// Use this for initialization
	new void Start () {
        base.Start();
        AdjustFacing();
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        CalculateDynamics();
        CheckImpact();
        Traslate();
    }

    protected override void CalculateDynamics()
    {
        base.CalculateDynamics();
        traslation = velocity * Time.deltaTime;
    }

    protected override void AdjustFacing()
    {
        base.AdjustFacing();
        transform.up = velocity.normalized;
    }

    protected override void CheckImpact()
    {
        base.CheckImpact();
        LayerMask impactMask = LayerMask.GetMask("Enemy");
        Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        float rayLength = bounds.extents.y - bounds.center.y + traslation.magnitude;
        Vector3 rayOrigin = transform.position - transform.right * bounds.extents.x * 2f / 3f;
        Vector3 rayDir = velocity.normalized;

        Debug.DrawRay(rayOrigin, rayDir, Color.yellow);
        Debug.DrawRay(rayOrigin + rayDir * rayLength, -rayDir, Color.magenta);
    }
}
