using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Stone : MonoBehaviour {

    SkeletonAnimator skeleton;
    Animator ani;

    // Use this for initialization
    void Awake () {
        skeleton = GetComponent<SkeletonAnimator>();
        ani = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {	
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Boss" || other.gameObject.tag == "Player" || other.gameObject.tag =="Ground")
        {
            ani.SetTrigger("Break");
            Destroy(gameObject, 0.3f); 
        }

    }
}
