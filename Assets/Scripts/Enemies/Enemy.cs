using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    protected const string ATTACKS_PATH = "Prefabs/Attacks/EnemyAttacks/";

    protected Player nearestPlayer;
    protected Vector3 npVector;

    protected Vector3 iAxis = Vector2.zero;

    [SerializeField]
    protected float sleep = 1f;
    [SerializeField]
    protected float SIGHT_RANGE = 8f;
    [SerializeField]
    protected float REFLEXES = 0.5f;
    protected float reflexesTimer = 0f;

    [SerializeField]
    protected float PUSH_FORCE = PlayerInput.JUMP_FORCE;

    [SerializeField]
    protected int CONTACT_DAMAGE = 1;

    protected bool awake = false;

    EnemySpawner spawner;

    // Use this for initialization
    protected new void Start () {
        base.Start();
        //InitializePlayerList();
	}
	
	// Update is called once per frame
	protected new void Update () {
        base.Update();
        //UdateNearestPlayer();
        PlayerList.GetNearestPlayer(transform.position, ref nearestPlayer, ref npVector);
        if (!awake)
            CheckAwake();
        else
        {
            if (sleep > 0)
                sleep -= Time.deltaTime;

            if (reflexesTimer > 0)
                reflexesTimer -= Time.deltaTime;
        }
        
    }

    void LateUpdate()
    {
        if (reflexesTimer <= 0f)
            reflexesTimer = REFLEXES;
    }

    /*void InitializePlayerList()
    {
        GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerGOs.Length];
        for (int i = 0; i < playerGOs.Length; i++)
            players[i] = playerGOs[i].GetComponent<Player>();

        nearestPlayer = players[0];
        npVector = nearestPlayer.transform.position - transform.position;
    }*/

    /*void UdateNearestPlayer()
    {
        if (players.Length > 1)
        {
            foreach (Player p in players)
            {
                Vector3 pVector = p.transform.position - transform.position;
                if (pVector.magnitude < npVector.magnitude)
                {
                    nearestPlayer = p;
                    npVector = pVector;
                }
            }
        }
        else {
            npVector = nearestPlayer.transform.position - transform.position;
        }
    }*/

    void CheckAwake()
    {
        awake = Vector3.Distance(nearestPlayer.transform.position, transform.position) <= SIGHT_RANGE;
    }



    protected virtual void MovementBehaviour()
    {

    }

    override public void GetHurt(int damage)
    {
        base.GetHurt(damage);
        if (health > 0)
        {
            Debug.Log(name + " remaining Health: " + health);
            mov2D.AddInstantTraslation(-mov2D.GetVelocity() * Time.deltaTime);
        }
        else {
            Die();
        }
    }

    protected void ResetReflexes()
    {
        reflexesTimer = REFLEXES;
    }

    override public void SpecialCollisions(RaycastHit2D hit)
    {
        base.SpecialCollisions(hit);
        GameObject other = hit.collider.gameObject;
        if (IsPlayer(other))
        {
            Player player = other.GetComponent<Player>();
            if (!player.IsGraced())
            {
                mov2D.SetVel(Vector3.zero);
                ResetReflexes();

                player.GetHurt(CONTACT_DAMAGE);
                player.BePushed(GetPushVector(transform.position.x, player.transform.position.x), PUSH_FORCE);
            }
        }
    }

    public void SetSpawner(EnemySpawner spwnr)
    {
        spawner = spwnr;
    }
    override public void Die()
    {
        if (spawner)
            spawner.SomeoneDied();
        base.Die();
    }
}
