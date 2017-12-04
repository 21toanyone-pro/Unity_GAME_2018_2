using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogColor : MonoBehaviour {

    public SpriteRenderer[] Fog = new SpriteRenderer[2];
    public Color32 night = new Color32(255, 152, 152, 60);
    Boss boss;
    // Use this for initialization
    void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }
	
	// Update is called once per frame
	void Update () {
        if (boss.paseCheck == 2)
        {
                Fog[0].color = Color32.Lerp(Fog[0].color, night, Time.deltaTime);
                Fog[1].color = Color32.Lerp(Fog[1].color, night, Time.deltaTime);
        }
    }
}
