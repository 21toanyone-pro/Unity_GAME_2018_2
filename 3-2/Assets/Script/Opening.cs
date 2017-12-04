using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening : MonoBehaviour {
    public GameObject Left;
    public GameObject Right;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 move = Vector3.zero;
        Vector3 move2 = Vector3.zero;
        move = Vector3.left;
        move2 = Vector3.right;

        Left.transform.position += move *10f* Time.deltaTime;
        Right.transform.position += move2 * 10f * Time.deltaTime;

        Destroy(gameObject, 3f);
    

    }
}
