using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Death : MonoBehaviour {

    Player player;
    public GameObject FogScene;
    bool DeadCheck;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
        
	}
	
	// Update is called once per frame
	void Update () {

        if(player.PlayerHP <= 0 && !DeadCheck)
        {
            Instantiate(FogScene, new Vector3(-8.889979f, 1.001822f, 0), Quaternion.identity);
            Invoke("End", 2.7f);
            DeadCheck = true;
        }
	}

    void End()
    {
        SceneManager.LoadScene("End");
    }
}
