using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour {

    static Player[] players;

    // Use this for initialization
    void Start () {
        InitializePlayerList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void InitializePlayerList()
    {
        GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerGOs.Length];
        for (int i = 0; i < playerGOs.Length; i++)
            players[i] = playerGOs[i].GetComponent<Player>();

        //nearestPlayer = players[0];
        //npVector = nearestPlayer.transform.position - transform.position;
    }
    
    public static void GetNearestPlayer(Vector3 refPos, ref Player nearestPlayer, ref Vector3 npVector)
    {
        if (players.Length > 1)
        {
            foreach (Player p in players)
            {
                Vector3 pVector = p.transform.position - refPos;
                if (pVector.magnitude < npVector.magnitude)
                {
                    nearestPlayer = p;
                    npVector = pVector;
                }
            }
        }
        else {
            nearestPlayer = players[0];
            npVector = nearestPlayer.transform.position - refPos;
        }
    }

    public static Player[] GetPlayerList()
    {
        return players;
    }
}
