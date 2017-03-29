using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    [SerializeField]
    int damage = 1;
    public int Damage
    {
        get { return damage; }
    }

    float pushForce = PlayerInput.JUMP_FORCE;
    public float PushForce
    {
        get { return pushForce; }
    }
}
