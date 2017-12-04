using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround_Manager : MonoBehaviour {
    Boss boss;
    public SpriteRenderer Fog;
    public Color32 night = new Color32(255, 101, 101, 255);

    // Use this for initialization
    void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
	}
	
	// Update is called once per frame
	void Update () {

        if (boss.paseCheck == 2)
        {

                Fog.color = Color32.Lerp(Fog.color, night, Time.deltaTime);
 
        }

    }
}
