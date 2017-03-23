using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

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

    

	// Use this for initialization
	protected void Start () {
		
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
}
