using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

    protected const float GRAVITY_VALUE = -19.81f;
    protected Vector3 GRAVITY = new Vector3(0f, GRAVITY_VALUE, 0f);

    [SerializeField]
    float lifeSpan;
    [SerializeField]
    float rangeDisplacement;

    protected float speed;
    protected int damage;

    protected GameObject owner;

    protected Vector3 velocity;
    protected Vector3 traslation;

    protected Bounds bounds;
    protected LayerMask impactMask;

	// Use this for initialization
	protected void Start () {
        InitializeImpactParams();
	}
	
	// Update is called once per frame
	protected void Update () {
        CheckLifeSpan();
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

    virtual protected void CalculateDynamics()
    {
        
    }

    virtual protected void AdjustFacing()
    {
        
    }

    virtual protected void CheckImpact()
    {
        
    }

    protected void Traslate()
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

    protected void HitSomeone(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            hit.collider.gameObject.GetComponent<Enemy>().GetHurt(damage);
        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hit.collider.gameObject.GetComponent<Player>().GetHurt(damage);
        }
    }

    virtual protected void InitializeImpactParams()
    {
        bounds = GetComponent<SpriteRenderer>().sprite.bounds;
    }

    protected string GetImpactLayerByOwnerLayer()
    {
        if (owner.layer == LayerMask.NameToLayer("Player"))
            return "Enemy";
        else
            return "Player";
    }
}
