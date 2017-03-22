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

    float speed;
    int damage;

    Vector3 velocity;
    Vector3 traslation;

    bool projectile;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckLifeSpan();
        CalculateDynamics();
        if(!velocity.Equals(Vector3.zero))
            transform.up = velocity.normalized;
        CheckImpact();
        Traslate();
	}

    void CheckLifeSpan()
    {
        if (lifeSpan > 0)
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

    void CheckImpact()
    {
        LayerMask impactMask;
        if (projectile)
        {
            impactMask = LayerMask.GetMask("Enemy", "Level");
        }
        else
        {
            impactMask = LayerMask.GetMask("Enemy");
        }
        //RAYCASTS
    }

    void Traslate()
    {
        transform.position += traslation;
    }

    public void SetAttack(Vector3 dir, float newSpeed, float newDamage)
    {
        speed = newSpeed;
        damage = Mathf.FloorToInt(newDamage);
        Debug.Log(damage);
        velocity = dir * speed;
        transform.up = dir;
        transform.position += dir * rangeDisplacement;
    }
}
