using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_State : MonoBehaviour {

    public Image PlayerHP;
    public Image PlayerST;
    //public Image PlayerST;
    Player player;
    float MaxHP = 100;
    float CurrentHP;

    float MaxST = 100;
    float CurrentST;

    // Use this for initialization
    void Awake () {
        PlayerHP.fillAmount = 1.0f;
        PlayerST.fillAmount = 1.0f;
        player = GameObject.Find("Player").GetComponent<Player>();
        

    }
	
	// Update is called once per frame
	void Update ()
    {
        CurrentHP = player.PlayerHP;
        PlayerHP.fillAmount = CurrentHP / MaxHP;

        CurrentST = player.PlayerST;
        PlayerST.fillAmount = CurrentST / MaxST;
    }
}
