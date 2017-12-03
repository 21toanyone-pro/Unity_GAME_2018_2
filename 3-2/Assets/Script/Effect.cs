using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Effect : MonoBehaviour {

    SkeletonAnimator Skeleton;
    float Alpha = 1f;
    float Alpha2 = 0f;
    float LastAlpha;
	// Use this for initialization
	void Start () {
        Skeleton = GetComponent<SkeletonAnimator>();
        
    }
    
    void Update()
    {
        Skeleton.GetComponent<SkeletonAnimator>().skeleton.a = Alpha;
        if(Alpha <=0)
        {
            Destroy(gameObject);
        }
    }

    void SlowSlowSLow()
    {
        StartCoroutine(SlowSlow());
    }

    IEnumerator SlowSlow()
    {
        yield return new WaitForSeconds(2f);
        do
        {
            Alpha -= 0.1f;
            yield return new WaitForSeconds(0.1f);

        } while (Alpha >= 0);

    }


    void Destroy()
    {
        Destroy(gameObject);
    }

    void Uprising_Dest()
    {
        Destroy(gameObject, 0.5f);
    }
}
