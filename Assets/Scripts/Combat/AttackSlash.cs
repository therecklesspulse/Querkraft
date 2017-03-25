using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlash : Attack {

    enum SlashShape
    {
        C,L,I
    }
    [SerializeField]
    SlashShape slashShape;

    bool exhausted = false;

	// Use this for initialization
	new void Start ()
    {
        base.Start();
        AdjustFacing();
	}
	
	// Update is called once per frame
	new void Update ()
    {
        base.Update();
        CalculateDynamics();
        if(!exhausted)
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
        int rayNum = 7;
        float raySpacing = bounds.extents.x * 2f / rayNum;
        float rayLength;
        Vector3 rayOrigin;
        Vector3 rayDir = velocity.normalized;

        RaycastHit2D nearestHit = new RaycastHit2D();
        nearestHit.distance = Mathf.Infinity;
        
        for(int i = 0; i < rayNum; i++)
        {
            rayOrigin = transform.position + (i - (rayNum - 1)/2f) * raySpacing * transform.right;
            rayLength = GetRayLengthBySlashShape(bounds, rayNum, i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, impactMask);
            if (hit)
            {
                if (hit.distance < nearestHit.distance)
                    nearestHit = hit;
            }
            Debug.DrawRay(rayOrigin, rayDir, Color.yellow);
            Debug.DrawRay(rayOrigin + rayDir * rayLength, -rayDir, Color.magenta);
        }

        if (nearestHit.distance < Mathf.Infinity)
        {
            exhausted = true;
            HitSomeone(nearestHit);
        }
    }

    float GetRayLengthBySlashShape(Bounds bounds, int rayNum, int i)
    {
        float rayLength;
        const float KIND_MARGIN = 0.1f;
        float maxLength = KIND_MARGIN + bounds.extents.y * transform.lossyScale.y - bounds.center.y + traslation.magnitude;
        float x = i / (rayNum-1f);

        switch (slashShape)
        {
            case SlashShape.C:
                rayLength = Mathf.Sqrt(1 - Mathf.Pow(2*x - 1, 2));
                break;
            case SlashShape.I:
                rayLength = 1f;
                break;
            case SlashShape.L:
                rayLength = Mathf.Sqrt(1-Mathf.Pow(x-1,2));
                break;
            default:
                rayLength = 1f;
                break;
        }

        rayLength *= maxLength;
        return rayLength;
    }

    override protected void InitializeImpactParams()
    {
        base.InitializeImpactParams();
        impactMask = LayerMask.GetMask(GetImpactLayerByOwnerLayer());
    }
}
