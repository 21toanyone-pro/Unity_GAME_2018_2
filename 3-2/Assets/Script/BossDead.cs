using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDead : MonoBehaviour {

    public Image img;
    public Color32 night = new Color32(0, 0, 0, 255);
    Boss boss;

    // Use this for initialization
    void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }
	
	// Update is called once per frame
	void Update () {
        if(boss.Boss_HP <= 0)
		img.color = Color32.Lerp(img.color, night, Time.deltaTime*5);
    }
}
