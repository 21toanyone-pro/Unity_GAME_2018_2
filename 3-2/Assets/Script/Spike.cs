using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Spike : MonoBehaviour {

  
    
    //SkeletonAnimation skeleton;
    SkeletonAnimator skeleton;
    Animator ani;
    Player player;

    // Use this for initialization
    void Awake () {
        skeleton = GetComponent<SkeletonAnimator>();
        ani = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //player.PlayerHP -= 20;
        }
    }


    public void DesStinger()
    {
        ani.SetTrigger("DownSpike");
    }

    public void EndStinger()
    {
        Destroy(gameObject);
    }
}
