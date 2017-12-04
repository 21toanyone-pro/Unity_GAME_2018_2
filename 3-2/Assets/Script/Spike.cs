using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Spike : MonoBehaviour {

    AudioSource Sound;

    //SkeletonAnimation skeleton;
    SkeletonAnimator skeleton;
    Animator ani;

    // Use this for initialization
    void Awake () {
        skeleton = GetComponent<SkeletonAnimator>();
        ani = GetComponent<Animator>();
        Sound = GetComponent<AudioSource>();
        Sound.Play();
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    private void OnTriggerEnter2D(Collider2D other)
    {

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
