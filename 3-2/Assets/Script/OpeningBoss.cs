using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningBoss : MonoBehaviour {

    public GameObject Bosses;
    bool Ok;

	// Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 move2 = Vector3.right;
        if(Ok)
            Bosses.transform.position += move2 * 15f * Time.deltaTime;
    }

    void OkOK()
    {
        Ok = true;
        Destroy(gameObject, 1.1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            OkOK();
        }
    }

}
