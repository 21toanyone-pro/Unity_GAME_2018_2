using UnityEngine;
using System;
using System.Collections;


public class CameraMove : MonoBehaviour {

    ///////////////////////////////
    //
    // Private Variable
    //
    ///////////////////////////////

    CameraFollow m_CameraFollow;
    Transform m_Transform;


    ///////////////////////////////
    //
    // Function
    //
    ///////////////////////////////

    void Awake()
    {
        m_CameraFollow = GetComponent<CameraFollow>();
        m_Transform = transform;
    }

    public void Move(Vector3 dstPosition, float moveTime, float delayTime, Action callback)
    {
        dstPosition.z = m_Transform.position.z;
        m_CameraFollow.enabled = false;
        StartCoroutine(UpdateMove(dstPosition, moveTime, delayTime, callback));
    }

    IEnumerator UpdateMove(Vector3 dstPosition, float moveTime, float delayTime, Action callback)
    {
        Vector3 srcPosition = transform.position;
        for (float rate = 0.0f; rate < 1.0f; rate += Time.deltaTime / moveTime)
        {
            transform.position = Vector3.Lerp(srcPosition, dstPosition, rate);
            yield return null;
        }
        yield return new WaitForSeconds(delayTime);
        for (float rate = 0.0f; rate < 1.0f; rate += Time.deltaTime / moveTime)
        {
            transform.position = Vector3.Lerp(dstPosition, srcPosition, rate);
            yield return null;
        }
        m_CameraFollow.enabled = true;
        if (callback != null)
            callback();
    }

}
