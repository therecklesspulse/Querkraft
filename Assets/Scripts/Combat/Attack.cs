using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    const float GRAVITY_VALUE = -19.81f;
    Vector3 GRAVITY = new Vector3(0f, GRAVITY_VALUE, 0f);

    [SerializeField]
    float lifeSpan;
    [SerializeField]
    float mass;
    [SerializeField]
    float rangeDisplacement;
    [SerializeField]
    bool isProjectile;

    bool stall = false;
    bool nailed = false;

    float speed;
    int damage;

    GameObject owner;

    Vector3 velocity;
    Vector3 traslation;

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckLifeSpan();
        if (!nailed)
        {
            CalculateDynamics();
            AdjustFacing();
            if (!stall)
                CheckImpact();
            Traslate();
        }
	}

    void CheckLifeSpan()
    {
        if (lifeSpan > 0 && owner)
            lifeSpan -= Time.deltaTime;
        else
        {
            // Make a propper Destroy
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void CalculateDynamics()
    {
        velocity += GRAVITY * mass * Time.deltaTime;
        traslation = velocity * Time.deltaTime;
    }

    void AdjustFacing()
    {
        if (!stall && !velocity.Equals(Vector3.zero))
            transform.up = velocity.normalized;
        else
            transform.up = Vector3.Lerp(transform.up, Vector3.down, Time.deltaTime);
    }

    void CheckImpact()
    {
        LayerMask impactMask;
        Vector3 rayOrigin;
        Vector3 rayDir = velocity.normalized;
        Bounds bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        float rayLength = bounds.extents.y - bounds.center.y + traslation.magnitude;
        if (isProjectile)
        {
            impactMask = LayerMask.GetMask("Player", "Enemy", "Level");
            rayOrigin = transform.position;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, impactMask);
            if (hit)
            {
                if(owner.layer == LayerMask.NameToLayer("Player") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
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
        else
        {
            impactMask = LayerMask.GetMask("Enemy");
            rayOrigin = transform.position - transform.right * bounds.extents.x*2f/3f;
            Debug.DrawRay(rayOrigin, rayDir, Color.yellow);
            Debug.DrawRay(rayOrigin + rayDir * rayLength, -rayDir, Color.magenta);
        }
    }

    void Traslate()
    {
        transform.position += traslation;
    }

    public void SetAttack(GameObject newOwner, Vector3 dir, float newSpeed, float newDamage)
    {
        owner = newOwner;
        speed = newSpeed;
        damage = Mathf.FloorToInt(newDamage);
        //Debug.Log(damage);
        velocity = dir * speed;
        transform.up = dir;
        transform.position += dir * rangeDisplacement;
    }
}
