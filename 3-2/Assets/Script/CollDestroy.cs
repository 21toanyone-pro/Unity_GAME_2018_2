using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollDestroy : MonoBehaviour {
    public bool BossRoom;
    public Collider2D coll;
    CameraFollow camera;
    public Collider2D ThisColl;
    public Image Name;

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
            camera.minXAndY.x = -10.9f;
            coll.enabled = true;
            ThisColl.enabled = false;
            Name.enabled = true;
            Invoke("Del", 2.3f);
        }
    }

    void Del()
    {
        Name.enabled = false;
    }
}
