using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollDestroy : MonoBehaviour {
    public bool BossRoom;
    public Collider2D coll;
    CameraFollow camera;
    public Collider2D ThisColl;

    private void Awake()
    {
        camera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        BossRoom = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            BossRoom = true;
            camera.minXAndY.x = -11.3f;
            coll.enabled = true;
            ThisColl.enabled = false;
        }
    }
}
