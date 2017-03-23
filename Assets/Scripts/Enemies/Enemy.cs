using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    protected const string ATTACKS_PATH = "Prefabs/Attacks/EnemyAttacks/";
    protected static Player[] players;
    protected Player nearestPlayer;
    protected Vector3 npVector;

    [SerializeField]
    protected Movement2D mov2D;

    protected Vector3 iAxis = Vector2.zero;

    [SerializeField]
    protected int health = 10;
    [SerializeField]
    protected float sleep = 1f;
    [SerializeField]
    protected float SIGHT_RANGE = 6f;
    [SerializeField]
    protected float REFLEXES = 0.5f;
    protected float reflexesTimer = 0f;

    // Use this for initialization
    protected void Start () {
        InitializePlayerList();

        if (!mov2D)
            mov2D = GetComponent<Movement2D>();
	}
	
	// Update is called once per frame
	protected void Update () {
        
        UdateNearestPlayer();
        if(sleep > 0)
            sleep -= Time.deltaTime;

        if (reflexesTimer > 0)
            reflexesTimer -= Time.deltaTime;
        
    }

    void LateUpdate()
    {
        if (reflexesTimer <= 0f)
            reflexesTimer = REFLEXES;
    }

    void InitializePlayerList()
    {
        GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerGOs.Length];
        for (int i = 0; i < playerGOs.Length; i++)
            players[i] = playerGOs[i].GetComponent<Player>();

        nearestPlayer = players[0];
        npVector = nearestPlayer.transform.position - transform.position;
    }

    void UdateNearestPlayer()
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
    }

    protected virtual void MovementBehaviour()
    {

    }

    public virtual void GetHurt(int damage)
    {
        health -= damage;
        if (health > 0)
            Debug.Log(name + " remaining Health: " + health);
        else {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
