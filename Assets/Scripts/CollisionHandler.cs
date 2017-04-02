using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {



    /*[SerializeField]
    int xRayNum = 7;
    [SerializeField]
    int yRayNum = 7;

    [SerializeField]
    float SKIN_WIDTH = 0.01f;

    struct HandmadeBounds
    {
        public Vector3 topLeft, topRight, botLeft, botRight;
    }

    HandmadeBounds handmadeBounds;*/

    [SerializeField]
    Collider2D c2d;
    Collider2D Collider
    {
        get { return c2d; }
    }

    float collisionRange = 0f;
    float CollisionRange
    {
        get { return collisionRange; }
    }

    Vector3 Center
    {
        get { return c2d.bounds.center; }
    }

    // Use this for initialization
    void Start () {
        if (!c2d)
            c2d = GetComponent<Collider2D>();

        collisionRange = CalculateCollisionRange();
	}


    // Update is called once per frame
    void Update () {
        //RaycastScan(handmadeBounds.topLeft, handmadeBounds.botLeft, handmadeBounds.topRight, xRayNum);
        //RaycastScan(handmadeBounds.topLeft, handmadeBounds.topRight, handmadeBounds.botLeft, yRayNum);
    }

    /*void RaycastScan(Vector3 initialP, Vector3 finalP, Vector3 initialTargetP, int rayNum)
    {
        float rayLength = Vector3.Distance(initialP, initialTargetP);
        float raySpacing = Vector3.Distance(initialP, finalP) / (rayNum-1);
        Vector3 rayDir = (initialTargetP - initialP).normalized;
        Vector3 scanDir = (finalP - initialP).normalized;

        for (int i = 0; i < rayNum; i++)
        {
            Vector3 rayOrigin = initialP + scanDir * i * raySpacing;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength);
            if (hit)
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.magenta);
            }
            else {
                Debug.DrawRay(rayOrigin, rayDir, Color.white);
            }
            
        }
    }*/

    void IntersectionScan()
    {

    }
    
    float CalculateCollisionRange()
    {
        float result = 0;

        if (c2d is BoxCollider2D || c2d is PolygonCollider2D)
        {
            Bounds bounds = c2d.bounds;
            result = Mathf.Sqrt(Mathf.Pow(bounds.extents.x * c2d.transform.lossyScale.x, 2) + Mathf.Pow(bounds.extents.y * c2d.transform.lossyScale.y, 2));

        }
        else if (c2d is CircleCollider2D)
        {
            CircleCollider2D cc = (CircleCollider2D)c2d;
            result = cc.radius * Mathf.Max(c2d.transform.lossyScale.x, c2d.transform.lossyScale.y);
        }

        return result;
    }

    public bool IsInCollisionRange(CollisionHandler other)
    {
        return Vector3.Distance(other.transform.position + other.Center, transform.position + Center) <= collisionRange + other.CollisionRange;
    }

    public bool CheckCollision(CollisionHandler other)
    {
        //Vector3 np = GetNearestPoint(this, other);
        //Vector3 onp = GetNearestPoint(other, this);
        Vector3 np = GetNearestPoint(other.transform.position);

        return other.IsPointInsideCollider(np); //|| IsPointInsideCollider(onp);
    }

    Vector3 GetNearestPoint(Vector3 to)
    {
        Vector3 trueCenter = transform.position + Center;
        Vector3 np = trueCenter;

        if (c2d is BoxCollider2D || c2d is PolygonCollider2D)
        {
            
        }
        else if (c2d is CircleCollider2D)
        {
            np = (to - trueCenter).normalized * collisionRange;
            if (Vector3.Distance(trueCenter, np) > Vector3.Distance(trueCenter, to))
                np = to;
        }

        return np;
    }

    public bool IsPointInsideCollider(Vector3 p)
    {
        Vector3 trueCenter = transform.position + Center;
        
        return (p.x > trueCenter.x - c2d.bounds.extents.x)
            && (p.x < trueCenter.x + c2d.bounds.extents.x)
            && (p.y > trueCenter.y - c2d.bounds.extents.y)
            && (p.y < trueCenter.y + c2d.bounds.extents.y);
    }
}
