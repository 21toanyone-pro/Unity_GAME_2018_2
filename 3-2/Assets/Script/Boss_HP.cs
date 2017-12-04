using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boss_HP : MonoBehaviour {

    public Image THis;
    public Image THis1;
    public Image THis2;
    public Image BossHP;
    Boss boss;
    float MaxHP = 1500;
    float CurrentHP;
    CollDestroy coll;

    // Use this for initialization
    void Awake () {
        coll = GameObject.Find("Check_Point").GetComponent<CollDestroy>();
        BossHP.fillAmount = 1.0f;
        boss = GameObject.Find("Boss").GetComponent<Boss>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 StopPos = new Vector3(transform.position.x, -15f, transform.position.z);
        if (coll.BossRoom)
        {
            THis.enabled = true;
            THis1.enabled = true;
            THis2.enabled = true;
        }

        CurrentHP = boss.Boss_HP;
        BossHP.fillAmount = CurrentHP / MaxHP;

    }
}
