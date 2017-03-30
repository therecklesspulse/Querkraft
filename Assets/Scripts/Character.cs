using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [SerializeField]
    public Movement2D mov2D;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected int health = 3;

    [SerializeField]
    protected float GRACE_TIME = 1.5f;
    protected float graceTime = 0f;
    protected const float FLICKING_FREQ = 30f;
    

    protected void Start()
    {
        if (!mov2D)
            mov2D = GetComponent<Movement2D>();
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Update()
    {
        if (IsGraced())
            Flicker();
    }

    virtual public void GetHurt(int damage)
    {
        if(!IsGraced())
            health -= damage;
    }

    protected void Flicker()
    {
        graceTime -= Time.deltaTime;
        if (graceTime <= 0)
            spriteRenderer.enabled = true;
        else
            spriteRenderer.enabled = Mathf.Sin(graceTime * FLICKING_FREQ) > 0;
    }

	virtual public void SpecialCollisions(RaycastHit2D hit)
    {

    }

    protected bool IsEnemy(GameObject o)
    {
        return o.layer == LayerMask.NameToLayer("Enemy");
    }
    protected bool IsPlayer(GameObject o)
    {
        return o.layer == LayerMask.NameToLayer("Player");
    }
    protected bool IsObstacle(GameObject o)
    {
        return o.layer == LayerMask.NameToLayer("Obstacle");
    }

    public bool IsGraced()
    {
        return graceTime > 0f;
    }

    protected Vector3 GetPushVector(float subjectX, float predicateX)
    {
        return new Vector3(Mathf.Sign(predicateX - subjectX), 2f, 0f).normalized;
    }

    virtual public void Die()
    {
        Destroy(gameObject);
    }
}
