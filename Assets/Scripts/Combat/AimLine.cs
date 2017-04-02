using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour {

    [SerializeField]
    Weapon weapon;
    [SerializeField]
    LineRenderer lineRenderer;

    // Use this for initialization
    void Start () {
        if (!weapon)
            weapon = GetComponent<Weapon>();
        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
