using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boss_HP : MonoBehaviour {

    public Image BossHP;
    Boss boss;
    float MaxHP = 1500;
    float CurrentHP;

    // Use this for initialization
    void Awake () {

        BossHP.fillAmount = 1.0f;
        boss = GameObject.Find("Boss").GetComponent<Boss>();
	}
	
	// Update is called once per frame
	void Update () {
        CurrentHP = boss.Boss_HP;
        BossHP.fillAmount = CurrentHP / MaxHP;
    }
}
