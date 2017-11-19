using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : MonoBehaviour {

    //Generally values between 2-4 are well for me.
    public int DelayInFrames = 4;
    //The sprite that we will follow
    public SpriteRenderer TargetSpriteRenderer;
    //The transparent sprite that will follow target
    public SpriteRenderer GhostSpriteRenderer;

    Vector3[] _positions;
    int[] _sortOrders;
    bool delayCompleted = false;
    //frame counter
    int count =0;

    void Awake()
    {
        //At the begining there are no sprites
        GhostSpriteRenderer.enabled = false;
        _positions = new Vector3[DelayInFrames];
        _sortOrders = new int[DelayInFrames];
    }
    //when wait time (enough frames passed) this will be set true.
    


    void Update()
    {
        //Record targets position and sorting order.
        //You may also record sprite if you need it.
        _positions[count] = TargetSpriteRenderer.transform.position;
        _sortOrders[count] = TargetSpriteRenderer.sortingOrder;

        //Increase frame counter
        count++;
        //If we waited enough; open the ghost image.
        if (count == DelayInFrames)
        {
            count = 0;
            if (!delayCompleted)
            {
                delayCompleted = true;
                GhostSpriteRenderer.enabled = true;
            }
        }
        //Now counter is referencing to oldest positions/orders we have; so use them:
        if (delayCompleted)
        {
            GhostSpriteRenderer.transform.position = _positions[count];
            GhostSpriteRenderer.sortingOrder = _sortOrders[count];
        }
    }
}
