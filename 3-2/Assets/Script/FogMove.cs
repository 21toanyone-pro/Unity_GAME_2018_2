using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMove : MonoBehaviour {

    public float ScrollSpeed = 0.5f;
    float Target;

	// Use this for initialization
	void Start () {
        
       
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 moveVec = new Vector3(56.9002f, -4.572422f, transform.position.z);
        Vector3 returnPos = new Vector3(-184.5002f, -4.6f, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, moveVec, Time.deltaTime);
        if(transform.position == moveVec)
        {
            transform.position = returnPos;
        }
    }
}
