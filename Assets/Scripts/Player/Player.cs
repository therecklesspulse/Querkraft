using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int health = 3;

	// Use this for initialization
	void Start () {
        Debug.Log("Health: " + health);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetHurt(int damage)
    {
        health -= damage;
        Debug.Log("Health: " + health);
        if (health <= 0) {
            Debug.Log("YOU LOSE");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
